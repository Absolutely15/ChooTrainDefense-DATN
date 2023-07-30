using UnityEngine;
using DG.Tweening;
using static NVTT.Utilities;
using Random = UnityEngine.Random;

public class TrajectoryArc : ITrajectory
{
	public float outerRange = 3;

	private Vector3 lastPos;

	private Vector3 controlPoint;

	private static Vector3 PlayerPos => Player.transform.position;

	// public override void Init(Projectile p)
	// {
	// 	base.Init(p);
	//
	// 	// originDirect = p.destination - p.startPos;
	// 	// p.transform.forward = originDirect;
	// 	/*controlPoint = Quaternion.AngleAxis(Random.Range(-180, 180), originDirect) * p.transform.right * outerRange;*/
	//
	// 	/*controlPoint = Random.Range(0.0f, 0.3f) * originDirect + Random.Range(0.0f, outerRange) * Vector3.up + Random.Range(-outerRange, outerRange) * Vector3.right;*/
	// }

	public override void DoMove(Vector3 startPos, Vector3 endPos, float t)
	{
		base.DoMove(startPos, endPos, t);
		lastPos = proj.transform.position;
		var newPos = DOCurve.CubicBezier.GetPointOnSegment(startPos, controlPoint, endPos, controlPoint, t);

		var newRot = Quaternion.LookRotation(newPos - lastPos);

		proj.transform.SetPositionAndRotation(newPos, newRot);
	}

	private void OnEnable()
	{
		GetRandomControlPoint();
	}

	private void GetRandomControlPoint()
	{
		controlPoint.x = Random.Range(-0.4f, 0.4f) + PlayerPos.x;
		controlPoint.y = Random.Range(1f, outerRange);
	}
}
