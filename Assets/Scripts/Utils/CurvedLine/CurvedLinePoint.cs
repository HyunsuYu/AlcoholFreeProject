using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


namespace RGBLineCoreLib.Functor
{
    /// <summary>
    /// Curved Line을 그리기 위한 Point
    /// </summary>
	/// <remarks>
	/// CurvedLineRenderer GameObject의 Child로 CurvedLinePoint를 가진 GameObject를 생성하여 사용한다
	/// </remarks>
    public class CurvedLinePoint : MonoBehaviour
	{
		[HideInInspector] public bool showGizmo = true;
		[HideInInspector] public float gizmoSize = 0.1f;
		[HideInInspector] public Color gizmoColor = new Color(1, 0, 0, 0.5f);

		private void OnDrawGizmos()
		{
			if (showGizmo == true)
			{
				Gizmos.color = gizmoColor;

				Gizmos.DrawSphere(this.transform.position, gizmoSize);
			}
		}

		private void OnDrawGizmosSelected()
		{
			CurvedLineRenderer curvedLine = this.transform.parent.GetComponent<CurvedLineRenderer>();

			if (curvedLine != null)
			{
				curvedLine.Update();
			}
		}
	}
}