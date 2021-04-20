using UnityEngine;
using UnityEngine.UI;

public class NetworkDisplayText : MonoBehaviour
{
	private Text textUI;

	void Start()
	{
		textUI = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (NetworkCommunicationController.singleton != null)
			textUI.text = "Bytes Received: " + NetworkCommunicationController.singleton.networkObject.Networker.BandwidthIn + "\nBytes Sent: " + NetworkCommunicationController.singleton.networkObject.Networker.BandwidthOut;
	}
}
