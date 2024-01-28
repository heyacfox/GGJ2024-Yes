using ReadSpeaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTSTester : MonoBehaviour
{

    TTSSpeaker speaker;
    // Start is called before the first frame update
    void Start()
    {
        TTS.Init();
        speaker = GetComponent<TTSSpeaker>();
        TTS.SayAsync("Hello, my name is ReadSpeaker. I am a text to speech engine.", speaker);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
