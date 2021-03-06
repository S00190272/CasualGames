﻿using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SignalRController : MonoBehaviour
{	
    public string endpoint = "http://localhost:2490/";
    public string hubName = "ChatHub";
    public string usersName;
	public string enteredUser;
    public bool isConnected = false;
    HubConnection connection;
    IHubProxy proxy;
    public ChatBox chat;

	// Use this for initialization
	void Start ()
    {
        DontDestroyOnLoad(this);
		
        ConnectToHub();

    }

    public void OnApplicationQuit()
    {
		connection.Stop();
    }

    
    
    public void ConnectToHub()
    {
        if (!string.IsNullOrEmpty(endpoint) && !string.IsNullOrEmpty(hubName))
        {
			
            connection = new HubConnection(endpoint);
            proxy = connection.CreateHubProxy(hubName);
            // connect to the player joined Hub and run the player joined method
            proxy.On("PlayerJoined", new Action<string>(OnPlayerJoined));
            proxy.On("RecieveMessage", new Action<string, string>(RecieveMessage));
            proxy.On("PlayerLeft", new Action<string>(PlayerLeft));
			isConnected = true;

            connection.Start().Wait();// Waits For task to complete
     
            
        }
    }

    public void JoinChat(string username)
    {
        if (isConnected)
        {		
			proxy.Invoke("Join", username);
			chat.OnPlayerJoined(username);
		}
    }

    public void LeftChat()
    {
        if (!isConnected)
        {			
            proxy.Invoke("Leave", usersName);
			PlayerLeft(usersName);			
        }
    }

    public void OnSendMessage(string message)
    {
        if (isConnected)
        {
            proxy.Invoke("SendMessageToOthers", usersName, message);
			//RecieveMessage(usersName, message);

		}
    }

	public void OnPlayerJoined(string user)
	{
		chat.OnPlayerJoined(user);
		//Debug.Log("OnplayerJoined");
	}    

    public void PlayerLeft(string username)
    {
		chat.PlayerLeft(username);
    }
    public void RecieveMessage(string username, string message)
    {
		chat.RecievedMessage(username, message);
    }
}
