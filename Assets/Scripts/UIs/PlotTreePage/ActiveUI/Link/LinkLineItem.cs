using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public sealed class LinkLineItem : MonoBehaviour
{
    private Guid m_linkID;

    [SerializeField] private Transform[] m_transform_LinePoints;

    private LineRenderer m_lineRenderer = null;
    private MeshCollider m_meshCollider = null;

    [SerializeField] private Material[] m_materials;


    public void Awake()
    {
        gameObject.SetActive(false);

        m_lineRenderer = GetComponent<LineRenderer>();
        m_meshCollider = GetComponent<MeshCollider>();
    }
    public void Update()
    {
        Mesh mesh = new Mesh();
        m_lineRenderer.BakeMesh(mesh, true);

        var verts = mesh.vertices;
        for(int index = 0; index < verts.Length; index++)
        {
            verts[index] = new Vector3(verts[index].x * 10.0f, verts[index].y * 10.0f, verts[index].z * 10.0f);
        }

        mesh.vertices = verts;

        m_meshCollider.sharedMesh = mesh;
    }

    internal Guid LinkID
    {
        get
        {
            return m_linkID;
        }
    }

    internal void SetFromLinkLineSpot(in Guid linkID)
    {
        PlotData.LinkData linkData = PlotDataBuffer.Instance.PlotData.Value.LinkTable[m_linkID];
        if(linkData.FromNodeID != Guid.Empty)
        {
            Vector3 fromSpotPos = Vector3.zero;
            switch (linkData.FromNodeType)
            {
                case PlotData.LinkNodeType.SceneNode:
                    fromSpotPos = PlotTreeActiveUIControl.Instance.GetSceneItem(linkData.FromNodeID).OutputParts[linkData.FromLinkCondition.Value.CurSceneNodeLinkCondition.Value.FromBranchIndex].SpotPos;
                    break;

                case PlotData.LinkNodeType.EnterEventNode:
                    fromSpotPos = PlotTreeActiveUIControl.Instance.GetEnterEventItem(linkData.FromLinkCondition.Value.CurEnterEventNodeLinkCondition.Value.EnterEventName).OutputPartSpot.SpotPos;
                    break;

                case PlotData.LinkNodeType.ScreenTransitionNode:
                    throw new InvalidOperationException("ScreenTransitionNode is not supported in LinkLineItem rendering.");
            }

            m_transform_LinePoints[0].position = fromSpotPos * 10.0f;
            m_transform_LinePoints[1].position = fromSpotPos * 10.0f + new Vector3(0.25f, 0.0f, 0.0f);
        }
    }
    internal void SetToLinkLineSpot(in Guid linkID)
    {
        PlotData.LinkData linkData = PlotDataBuffer.Instance.PlotData.Value.LinkTable[m_linkID];
        if(linkData.ToNodeID != Guid.Empty)
        {
            Vector3 toSpotPos = Vector3.zero;
            switch (linkData.ToNodeType)
            {
                case PlotData.LinkNodeType.SceneNode:
                    toSpotPos = PlotTreeActiveUIControl.Instance.GetSceneItem(linkData.ToNodeID).InputPartSpot.SpotPos;
                    break;

                case PlotData.LinkNodeType.EnterEventNode:
                    throw new InvalidOperationException("EnterEventNode is not supported in LinkLineItem rendering.");

                case PlotData.LinkNodeType.ScreenTransitionNode:
                    toSpotPos = PlotTreeActiveUIControl.Instance.GetScreenTransitionItem(linkData.ToLinkCondition.Value.CurScreenTransitionNodeLinkCondition.Value.ScreenTransitionIndex).InputPartSpot.SpotPos;
                    break;
            }

            m_transform_LinePoints[2].position = toSpotPos * 10.0f + new Vector3(-0.25f, 0.0f, 0.0f);
            m_transform_LinePoints[3].position = toSpotPos * 10.0f;
        }
    }

    internal void Render(in Guid linkID)
    {
        gameObject.SetActive(false);

        m_linkID = linkID;

        SetFromLinkLineSpot(linkID);
        SetToLinkLineSpot(linkID);

        if (PlotTreeActiveUIControl.Instance.CurSelectedLinkID == m_linkID)
        {
            m_lineRenderer.material = m_materials[1];
        }
        else
        {
            m_lineRenderer.material = m_materials[0];
        }

        gameObject.SetActive(true);

        Invoke("SetInitedState", 0.25f);
    }
}