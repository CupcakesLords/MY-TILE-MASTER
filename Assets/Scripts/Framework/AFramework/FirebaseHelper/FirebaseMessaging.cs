﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFramework.FirebaseService
{
    public class FirebaseMessaging : MonoBehaviour
    {
#if USE_FIREBASE && USE_FIREBASE_MESSAGING
        public static System.Action<Firebase.Messaging.TokenReceivedEventArgs> EventOnTokenReceived;
        public static System.Action<Firebase.Messaging.MessageReceivedEventArgs> EventOnMessageReceived;
        public static string FirebaseMessagingToken { get; protected set; }

        void Start()
        {
            FirebaseInstance.ChecAndTryInit(Init);
        }

        void Init()
        {
            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        }

        void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
        {
            FirebaseMessagingToken = token.Token;
            if (EventOnTokenReceived != null) EventOnTokenReceived(token);
        }

        void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
        {
            if (EventOnMessageReceived != null) EventOnMessageReceived(e);
        }
#endif
    }
}