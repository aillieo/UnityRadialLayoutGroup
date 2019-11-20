
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AillieoUtils.UI
{
    public class RadialLayoutGroup : LayoutGroup
    {
        public enum Direction { Clockwise = 0, Counterclockwise = 1, Bidirectional = 2 }

        public enum ConstraintMode { Interval = 0, Range = 1 }

        [SerializeField]
        private ConstraintMode mAngleConstraint;
        public ConstraintMode AngleConstraint { get { return mAngleConstraint; } set { SetProperty(ref mAngleConstraint, value); } }

        [SerializeField]
        private ConstraintMode mRadiusConstraint;
        public ConstraintMode RadiusConstraint { get { return mRadiusConstraint; } set { SetProperty(ref mRadiusConstraint, value); } }

        [SerializeField]
        private Direction mLayoutDir;
        public Direction LayoutDir { get { return mLayoutDir; } set { SetProperty(ref mLayoutDir, value); } }

        [SerializeField]
        private float mRadiusStart;
        public float RadiusStart { get { return mRadiusStart; } set { SetProperty(ref mRadiusStart, value); } }

        [SerializeField]
        private float mRadiusDelta;
        public float RadiusDelta { get { return mRadiusDelta; } set { SetProperty(ref mRadiusDelta, value); } }

        [SerializeField]
        private float mRadiusRange;
        public float RadiusRange { get { return mRadiusRange; } set { SetProperty(ref mRadiusRange, value); } }

        [SerializeField]
        private float mAngleDelta;
        public float AngleDelta { get { return mAngleDelta; } set { SetProperty(ref mAngleDelta, value); } }

        [SerializeField]
        private float mAngleStart;
        public float AngleStart { get { return mAngleStart; } set { SetProperty(ref mAngleStart, value); } }

        [SerializeField]
        private float mAngleCenter;
        public float AngleCenter { get { return mAngleCenter; } set { SetProperty(ref mAngleCenter, value); } }

        [SerializeField]
        private float mAngleRange;
        public float AngleRange { get { return mAngleRange; } set { SetProperty(ref mAngleRange, value); } }

        [SerializeField]
        private bool mChildRotate = false;
        public bool ChildRotate { get { return mChildRotate; } set { SetProperty(ref mChildRotate, value); } }

        public override void CalculateLayoutInputVertical()
        {
        }
        public override void CalculateLayoutInputHorizontal()
        {
        }

        public override void SetLayoutHorizontal()
        {
            CalculateChildrenPositions();
        }

        public override void SetLayoutVertical()
        {
            CalculateChildrenPositions();
        }

        private List<RectTransform> childList = new List<RectTransform>();
        private List<ILayoutIgnorer> ignoreList = new List<ILayoutIgnorer>();
        private void CalculateChildrenPositions()
        {
            this.m_Tracker.Clear();

            childList.Clear();

            for (int i = 0; i < this.transform.childCount; ++i)
            {
                RectTransform rect = this.transform.GetChild(i) as RectTransform;

                if(!rect.gameObject.activeSelf)
                {
                    continue;
                }

                ignoreList.Clear();
                rect.GetComponents(ignoreList);
                if (ignoreList.Count == 0)
                {
                    childList.Add(rect);
                    continue;
                }

                for (int j = 0; j < ignoreList.Count; j++)
                {
                    if (!ignoreList[j].ignoreLayout)
                    {
                        childList.Add(rect);
                        break;
                    }
                }

                ignoreList.Clear();
            }

            EnsureParameters(childList.Count);

            for (int i = 0; i < childList.Count; ++i)
            {
                var child = childList[i];
                float delta = i * mAngleDelta;
                float angle = LayoutDir == Direction.Clockwise ? mAngleStart - delta : mAngleStart + delta;
                ProcessOneChild(child, angle, mRadiusStart + (i * mRadiusDelta));
            }

            childList.Clear();
        }

        private void EnsureParameters(int childCount)
        {
            EnsureAngleParameters(childCount);
            EnsureRadiusParameters(childCount);
        }

        private void EnsureAngleParameters(int childCount)
        {
            int intervalCount = childCount - 1;
            switch (LayoutDir)
            {
            case Direction.Clockwise:
                if (AngleConstraint == ConstraintMode.Interval)
                {
                    // mAngleDelta mAngleStart
                    this.mAngleRange = intervalCount * this.mAngleDelta;
                }
                else
                {
                    // mAngleRange mAngleStart
                    if (intervalCount > 0)
                    {
                        this.mAngleDelta = this.mAngleRange / intervalCount;
                    }
                    else
                    {
                        this.mAngleDelta = 0;
                    }
                }
                break;
            case Direction.Counterclockwise:
                if (AngleConstraint == ConstraintMode.Interval)
                {
                    // mAngleDelta mAngleStart
                    this.mAngleRange = intervalCount * this.mAngleDelta;
                }
                else
                {
                    // mAngleRange mAngleStart
                    if (intervalCount > 0)
                    {
                        this.mAngleDelta = this.mAngleRange / intervalCount;
                    }
                    else
                    {
                        this.mAngleDelta = 0;
                    }
                }
                break;
            case Direction.Bidirectional:
                if (AngleConstraint == ConstraintMode.Interval)
                {
                    // mAngleDelta mAngleCenter
                    this.mAngleRange = intervalCount * this.mAngleDelta;
                }
                else
                {
                    // mAngleRange mAngleCenter
                    if (intervalCount > 0)
                    {
                        this.mAngleDelta = this.mAngleRange / intervalCount;

                    }
                    else
                    {
                        this.mAngleDelta = 0;
                    }
                }
                this.mAngleStart = this.mAngleCenter - mAngleRange * 0.5f;
                break;
            }
        }


        private void EnsureRadiusParameters(int childCount)
        {
            int intervalCount = childCount - 1;
            switch (LayoutDir)
            {
            case Direction.Clockwise:
                if (RadiusConstraint == ConstraintMode.Interval)
                {
                    // mRadiusDelta mRadiusStart
                    this.mRadiusRange = intervalCount * this.mRadiusDelta;
                }
                else
                {
                    // mRadiusRange mRadiusStart
                    if (intervalCount > 0)
                    {
                        this.mRadiusDelta = mRadiusRange / intervalCount;
                    }
                    else
                    {
                        this.mRadiusDelta = 0;
                    }
                }
                break;
            case Direction.Counterclockwise:
            case Direction.Bidirectional:
                if (RadiusConstraint == ConstraintMode.Interval)
                {
                    // mRadiusDelta mRadiusStart
                    this.mRadiusRange = intervalCount * this.mRadiusDelta;
                }
                else
                {
                    // mRadiusRange mRadiusStart
                    if (intervalCount > 0)
                    {
                        this.mRadiusDelta = mRadiusRange / intervalCount;
                    }
                    else
                    {
                        this.mRadiusDelta = 0;
                    }
                }
                break;
            }
        }

        private static readonly Vector2 center = new Vector2(0.5f, 0.5f);

        private void ProcessOneChild(RectTransform child, float angle, float radius)
        {
            Vector3 pos = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad),
                0.0f);
            child.localPosition = pos * radius;

            DrivenTransformProperties drivenProperties =
                DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.Rotation | DrivenTransformProperties.Pivot;
            m_Tracker.Add(this, child, drivenProperties);

            child.anchorMin = center;
            child.anchorMax = center;
            child.pivot = center;

            if(this.ChildRotate)
            {
                child.localEulerAngles = new Vector3(0, 0, angle);
            }
            else
            {
                child.localEulerAngles = Vector3.zero;
            }

        }

//#if UNITY_EDITOR
//        protected override void OnValidate()
//        {
//            base.OnValidate();
//            CalculateChildrenPositions();
//        }
//#endif

    }
}
