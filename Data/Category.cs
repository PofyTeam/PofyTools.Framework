﻿using Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace PofyTools
{

    [System.Serializable]
    public class CategoryDefinition : Definition
    {
        public CategoryDefinition(string key)
        {
            this.id = key;
        }

        [Header("Base Categories")]
        [FormerlySerializedAs("baseCategories")]
        public List<string> baseIds = new List<string>();

    }
    [System.Obsolete ("No real extension only utility stuff. Should be removed.")]
    public class CategoryDefinitionSet : DefinitionSet<CategoryDefinition>
    {
        public CategoryDefinitionSet(string fullPath, string filename, bool scramble = false, bool encode = false, string extension = "") : base(fullPath, filename, scramble, encode, extension)
        {
        }

        public void Optimize()
        {
            for (int i = this._content.Count - 1; i >= 0; i--)
            {
                var element = this._content[i];
                if (string.IsNullOrEmpty(element.id))
                {
                    this._content.RemoveAt(i);
                    continue;
                }

                element.baseIds.RemoveAll(x => string.IsNullOrEmpty(x));
                element.baseIds.Remove(element.id);
            }

            DataUtility.OptimizeDefinitions(this._content);
        }

        public override void Save()
        {
            Optimize();
            base.Save();
        }
    }

    public class CategoryData : DefinableData<CategoryDefinition>
    {
        public CategoryData(CategoryDefinition definition) : base(definition) { }

        #region API
        public void AddSubcategory(CategoryData data)
        {
            this._subcategories.AddOnce(data);
            foreach (var cat in this._supercategories)
            {
                cat.AddSubcategory(data);
            }
        }

        public void AddSupercategory(CategoryData data)
        {
            this._supercategories.AddOnce(data);
            foreach (var cat in this._subcategories)
            {
                cat.AddSupercategory(data);
            }
        }

        public bool IsCategoryOf(string category)
        {
            if (this.id == category)
                return true;

            foreach (var cat in this.descriptor.supercategoryIds)
            {
                if (category == cat)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region Runtime Data
        public List<CategoryData> _subcategories = new List<CategoryData>();
        public List<CategoryData> _supercategories = new List<CategoryData>();
        #endregion

        public Descriptor descriptor = new Descriptor();

        [System.Serializable]
        public class Descriptor
        {
            public string id;
            public List<string> subcategoryIds = new List<string>();
            public List<string> supercategoryIds = new List<string>();

        }
    }

    public class CategoryDataSet : DataSet<string, CategoryData>
    {
        public CategoryDataSet(DefinitionSet<CategoryDefinition> categoryDefinitionSet)
        {
            Initialize(categoryDefinitionSet.GetContent());
        }

        public CategoryDataSet(List<CategoryDefinition> _content)
        {
            Initialize(_content);
        }

        /// <summary>
        /// Topmost categories.
        /// </summary>
        public List<CategoryData> rootCategories = new List<CategoryData>();

        /// <summary>
        /// Bottommost categories.
        /// </summary>
        public List<CategoryData> leafCategory = new List<CategoryData>();

        public bool Initialize(List<CategoryDefinition> categoryDefs)
        {
            if (!this.IsInitialized)
            {
                this.content = new Dictionary<string, CategoryData>(categoryDefs.Count);

                foreach (var category in categoryDefs)
                {
                    CategoryData data = new CategoryData(category);

                    //list
                    this._content.Add(data);

                    //dictionary
                    this.content[data.id] = data;

                    if (category.baseIds.Count == 0)
                    {
                        this.rootCategories.Add(data);
                    }
                }

                Initialize();

                return true;
            }
            return false;
        }

        public override bool Initialize()
        {
            if (!this.IsInitialized)
            {
                //find subcategories
                foreach (var data in this._content)
                {
                    foreach (var baseCategory in data.Definition.baseIds)
                    {
                        CategoryData baseData;
                        if (this.content.TryGetValue(baseCategory, out baseData))
                        {
                            baseData.AddSubcategory(data);
                        }

                        data.AddSupercategory(baseData);
                    }
                }

                //find leafs
                foreach (var data in this._content)
                {
                    if (data._subcategories.Count == 0)
                        this.leafCategory.Add(data);
                }

                //Propagate rootcategories
                foreach (var data in this.rootCategories)
                {
                    foreach (var sub in data._subcategories)
                    {
                        sub.AddSupercategory(data);
                    }
                }

                //Propagate leafs
                foreach (var data in this.leafCategory)
                {
                    foreach (var super in data._supercategories)
                    {
                        super.AddSubcategory(data);
                    }
                }

                foreach (var data in this._content)
                {
                    data.descriptor.id = data.id;
                    foreach (var sub in data._subcategories)
                    {
                        data.descriptor.subcategoryIds.Add(sub.id);
                    }
                    foreach (var sup in data._supercategories)
                    {
                        data.descriptor.supercategoryIds.Add(sup.id);
                    }
                }

                this.IsInitialized = true;
                return true;
            }
            return false;
        }

        public List<CategoryData.Descriptor> GetDescriptors()
        {
            List<CategoryData.Descriptor> result = new List<CategoryData.Descriptor>(this._content.Count);

            foreach (var data in this._content)
            {
                result.Add(data.descriptor);
            }

            return result;
        }
    }

    public interface ICategorizable
    {
        CategoryData CategoryData { get; }
        void Categorize(CategoryData data);
    }
}