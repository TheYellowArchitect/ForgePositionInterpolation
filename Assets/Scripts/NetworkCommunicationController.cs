using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using BeardedManStudios.Forge.Networking;

public class NetworkCommunicationController : NetworkCommunicationControllerBehavior
{

	public static NetworkCommunicationController singleton = null;

	public NetworkPositionInterpolationController positionController;

	//Notifies everyone and everything, that it is setup on the network, ready to work!
	protected override void NetworkStart()
	{
		base.NetworkStart();

		MainThreadManager.Run(() => Debug.Log("Network Started!"));

		singleton = this;

		positionController = FindObjectOfType<NetworkPositionInterpolationController>();

		DontDestroyOnLoad(this.gameObject);
	}

	public override void ReceiveHostPosition(RpcArgs args)
	{
		positionController.UpdateLatestBallPosition(args);
	}

	public void SendHostPosition(double timestamp, Vector2 position)
	{
		networkObject.SendRpc(RPC_RECEIVE_HOST_POSITION, Receivers.Others, timestamp, position);
	}
}
