﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PofyTools
{
    //[System.Serializable]
    public struct Cone
    {
        //X is cone angle Y is cone height 
        public Vector2 Data
        {
            get; private set;
        }

        //from tip to base center
        public Vector3 Direction
        {
            get; private set;
        }

        //tip world position
        public Vector3 TipPosition
        {
            get; private set;
        }

        #region Constructors

        public Cone(Vector3 tipPosition, Vector3 direction, Vector2 data)
        {
            this.TipPosition = tipPosition;
            this.Direction = direction;
            this.Data = data;
        }

        public Cone(float tipAngle = 1f, float coneHeight = 1f, Vector3 tipPosition = default(Vector3), Vector3 direction = default(Vector3))
        {
            this.TipPosition = tipPosition;
            this.Direction = direction;

            this.Data = new Vector2(tipAngle, coneHeight);
        }

        #endregion

        #region API
        /// <summary>
        /// Check if the cone contains world space point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Vector3 point)
        {
            if ((this.TipPosition - point).magnitude < Data.y)
            {
                return InfinityContains(point);
            }

            return false;
        }

        /// <summary>
        /// Checks if infinity cone contains world space point. (Does not check against distance)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool InfinityContains(Vector3 point)
        {
            Vector3 directionToPoint = point - this.TipPosition;
            if (Vector3.Angle(directionToPoint, this.Direction) < this.Data.x)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Set direction to the point
        /// </summary>
        /// <param name="point"></param>
        public void LookAt(Vector3 point)
        {
            this.Direction = (point - this.TipPosition).normalized;
        }

        /// <summary>
        /// Sets new direction.
        /// </summary>
        /// <param name="direction"></param>
        public void SetDirection(Vector3 direction)
        {
            this.Direction = direction.normalized;
        }

        public void SetTipPosition(Vector3 worldPosition)
        {
            this.TipPosition = worldPosition;
        }

        public void SetAtTransform(Transform transform)
        {
            SetDirection(transform.forward);
            SetTipPosition(transform.position);
        }

        public void SetHeight(float height)
        {
            Vector2 newData = this.Data;
            newData.y = height;
            this.Data = newData;
        }

        public void SetAngle(float angleInDegrees)
        {
            Vector2 newData = this.Data;
            newData.x = angleInDegrees;
            this.Data = newData;
        }

        public void SetData(float angleInDegrees, float height)
        {
            Vector2 newData = new Vector2(angleInDegrees, height);
            this.Data = newData;
        }

        public void Draw(Color color)
        {
            Color tempColor = Gizmos.color;
            Gizmos.color = color;

            Gizmos.DrawWireSphere(this.TipPosition, 0.05f);
            float rad = this.Data.y * Mathf.Tan(this.Data.x * Mathf.Deg2Rad);

            Vector3 endPoint = this.TipPosition + (this.Direction * this.Data.y);
            Gizmos.DrawLine(this.TipPosition, endPoint);
            Gizmos.DrawWireSphere(endPoint, rad);

            //Gizmos.DrawFrustum (this.tipPosition, this.data.x, this.data.y, 0.1f, 1f);
            Gizmos.color = tempColor;
        }

        
        public Vector3 GetRandomPointInisde()
        {
            //    Vector3 result = default (Vector3);
            //    float height = (Random.Range (0, this.Data.y));
            //    Vector3 distance = this.TipPosition + Direction * height;
            //    float radius = GetRadiusAtDistanceFromTip (height);
            //    radius = Random.Range (0, radius);

            //    return result;

            //get base circle radius & center point
            float baseCircleRadius = Mathf.Tan(this.Data.x) * this.Data.y;
            Vector3 baseCircleCenter = this.TipPosition + this.Direction;
            //find up & right axis, normal to a Direction (from tip to base circle center)
            Vector3 baseCircleUpNormalized = Vector3.Cross(this.Direction, Vector3.up).normalized;
            Vector3 baseCircleRightNormalized = Vector3.Cross(this.Direction.normalized, baseCircleUpNormalized).normalized;
            //get random point inside base cirle (local pos)
            Vector2 randomPointOnBaseCircleLocal = Random.insideUnitCircle * baseCircleRadius;
            //find points on up & right axis, defined by local random point pos
            Vector3 pointOnBaseCircleRightAxis = baseCircleRightNormalized * randomPointOnBaseCircleLocal.x;
            Vector3 pointOnBaseCircleUpAxis = baseCircleUpNormalized * randomPointOnBaseCircleLocal.y;
            Vector3 directionBetweenAxisPonts = pointOnBaseCircleRightAxis - pointOnBaseCircleUpAxis;
            Vector3 halfPointBetweenAxisPonts = pointOnBaseCircleRightAxis + directionBetweenAxisPonts / 2f;
            //get random point on base circle (global pos) & direction from tip 
            Vector3 randomPointOnBaseCircleGlobal = baseCircleCenter + (halfPointBetweenAxisPonts - baseCircleCenter) * 2f;
            Vector3 directionToRandomPointOnBaseCircle = randomPointOnBaseCircleGlobal - this.TipPosition;
            //get final random point inside cone
            return this.TipPosition + directionToRandomPointOnBaseCircle * Random.Range(0f, 1f);
        }

        public float GetRadiusAtDistanceFromTip(float distance)
        {
            return distance * Mathf.Tan(this.Data.x * Mathf.Deg2Rad);
        }

        #endregion


    }
}
