using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Http;
using System.IO;
//using Twilio;
//using Twilio.Rest.Api.V2010.Account;

public class SendWhatsAppMessage : MonoBehaviour
{
    //string yourId = "ADbgDnrpSkyOw54/ZXip2HNvbW5hdGhfZG90X2Nob3dkaHVyeTg2X2F0X2dtYWlsX2RvdF9jb20=";
    //string yourMobile = "+919008790806";
    //string yourMessage = "What a great day.";

    // Start is called before the first frame update
    void Start()
    {
        const string accountSid = "AC2e279df7ed26dada8898b8ca4149508b";
        const string authToken = "6aabfc50a1fe94e554e8d37f94c9b5da";
        
        Twilio.TwilioClient.Init(accountSid, authToken);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SendTestMessage()
    {
        var message = Twilio.Rest.Api.V2010.Account.MessageResource.Create(
             body: "This is the ship that made the Kessel Run in fourteen parsecs?",
             from: new Twilio.Types.PhoneNumber("+15017122661"),
             to: new Twilio.Types.PhoneNumber("+919008790806")
         );
    }
}
