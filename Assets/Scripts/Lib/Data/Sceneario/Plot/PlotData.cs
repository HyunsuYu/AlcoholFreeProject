using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public struct PlotData
{
    public enum LinkNodeType
    {
        SceneNode,
        EnterEventNode,
        ScreenTransitionNode
    }

    public struct LinkData
    {
        public enum LinkConditionCompareType
        {
            Equal,
            NotEqual,
            GreaterThan,
            LessThan,
            GreaterThanOrEqual,
            LessThanOrEqual
        }
        public enum LinkConditionChangeType
        {
            Addition,
            Subtraction,
            Multiplication,
            Division,
            Modulus,
            Set
        }

        public struct LinkCondition
        {
            public struct SceneNodeLinkCondition
            {
                /// <summary>
                /// 만약 Scene의 마지막 Frame이 Branch Frame이 아니라면 0으로 고정, Branch Frame이라면 Branch 선택지의 Index를 의미
                /// </summary>
                public int FromBranchIndex;
            }
            public struct EnterEventNodeLinkCondition
            {
                public string EnterEventName;

                //public Dictionary<string, Tuple<LinkConditionCompareType, int>> GlobalVariableConditionTable;
            }
            public struct ScreenTransitionNodeLinkCondition
            {
                public int ScreenTransitionIndex;
            }


            public SceneNodeLinkCondition? CurSceneNodeLinkCondition;
            public EnterEventNodeLinkCondition? CurEnterEventNodeLinkCondition;
            public ScreenTransitionNodeLinkCondition? CurScreenTransitionNodeLinkCondition;
        }


        public LinkNodeType FromNodeType;
        public LinkNodeType ToNodeType;

        public Guid FromNodeID;
        public Guid ToNodeID;

        public LinkCondition? FromLinkCondition;
        public LinkCondition? ToLinkCondition;

        public Dictionary<string, Tuple<LinkConditionCompareType, int>> GlobalVariableConditionTable;
        public Dictionary<string, Tuple<LinkConditionChangeType, int>> PostEffectTable;
    }


    public Dictionary<Guid, SceneData> SceneTable;
    public Dictionary<Guid, LinkData> LinkTable;

    public Dictionary<string, SerializableVector3> EnterEventTable;
    public List<SerializableVector3> ScreenTransitionNodePos;


    internal static Guid EnterEventNodeID
    {
        get
        {
            return new Guid("00000000-0000-0000-0000-000000000001");
        }
    }
    internal static Guid ScreenTransitionNodeID
    {
        get
        {
            return new Guid("00000000-0000-0000-0000-000000000002");
        }
    }
}