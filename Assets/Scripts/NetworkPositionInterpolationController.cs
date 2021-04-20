using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using UnityEngine;

public struct Snapshot
{
	public Vector2 position;
	public double timestamp;
}

public class NetworkPositionInterpolationController : MonoBehaviour
{
	public static NetworkPositionInterpolationController singleton = null;

	[Header("Position Snapshot")]

	[Tooltip("0.05 seconds is 50 milliseconds")]
	public double timePerSnapshot = 0.050d;

	[Tooltip("[0] is current, [1] is the one we interpolate towards, [2] and [3] are the 2 next ones")]
	public List<Snapshot> snapshots = new List<Snapshot>();

	private double timePassedSinceLastPosition = 0d;
	private double timePassedSinceLastPositionTotal = 0d;
	public Vector2 lerpResultPosition;
	public double lerpRate;

	public Vector2 lastReceivedBallPosition;

	public GameObject ballGameObject;
	public Rigidbody2D ballRigidbody;

	void Start()
	{
		//Setting our singleton
		singleton = this;
	}

	public void UpdateLatestBallPosition(RpcArgs args)
	{
		double receivedSnapshotTime = args.GetNext<double>();
		Vector2 receivedBallPosition = args.GetNext<Vector2>();

		snapshots.Add(new Snapshot { timestamp = receivedSnapshotTime, position = receivedBallPosition});

		//If 2nd snapshot, put the timestamp to start properly!
		if (snapshots.Count == 2)
			timePassedSinceLastPosition = snapshots[0].timestamp;
	}

	public void Update()
	{
		if (NetworkCommunicationController.singleton == null)
			return;

		//If host, move and send
		if (NetworkCommunicationController.singleton.networkObject.IsServer)
		{
			//Move
			ballRigidbody.velocity = new Vector2(Input.GetAxis("Horizontal") * 3, Input.GetAxis("Vertical") * 3);


			Debug.Log("Time.unscaledDeltaTime: " + Time.unscaledDeltaTime);

			//Update the timestamps below, and send by timer.
			timePassedSinceLastPosition = timePassedSinceLastPosition + Time.unscaledDeltaTime;
			timePassedSinceLastPositionTotal = timePassedSinceLastPositionTotal + Time.unscaledDeltaTime;
			if (timePassedSinceLastPosition >= timePerSnapshot || timePassedSinceLastPositionTotal == 0d)
			{
				timePassedSinceLastPosition = 0;
				NetworkCommunicationController.singleton.SendHostPosition(timePassedSinceLastPositionTotal, new Vector2(ballGameObject.transform.position.x, ballGameObject.transform.position.y));
			}
		}
		else
		{
			InterpolateToFinalPosition();
			timePassedSinceLastPosition = timePassedSinceLastPosition + Time.unscaledDeltaTime;
		}
	}

	public void InterpolateToFinalPosition()
	{

		if (snapshots.Count < 2)
		{
			Debug.LogWarning("Not enough snapshots, to do any interpolation.");
			return;
		}

		while (snapshots.Count > 2 && timePassedSinceLastPosition > snapshots[1].timestamp)
		{
			snapshots.RemoveAt(0);
		}

		Debug.Log("Snapshots: " +snapshots.Count);
		Debug.Log("Final Snapshot: " + snapshots[snapshots.Count - 1].timestamp + " X " + snapshots[snapshots.Count - 1].position.x);

		lerpRate = (timePassedSinceLastPosition - snapshots[0].timestamp) / (snapshots[1].timestamp - snapshots[0].timestamp);
		lerpResultPosition = Vector2.Lerp(snapshots[0].position, snapshots[1].position, (float)lerpRate);

		//This is for a VERY rare bug (only on high pings) where lerpResultPosition is NaN... like, wtf?!
		if (float.IsNaN(lerpResultPosition.x))
			return;

		//Take the interpolated position and put it onto the ball
		ballGameObject.transform.position = new Vector3(lerpResultPosition.x, lerpResultPosition.y, 0);

	}
}
