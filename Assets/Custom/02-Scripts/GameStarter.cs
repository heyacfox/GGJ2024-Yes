using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//On game start, the StartPlaying function should be called on the PlayDialogueSequence script
public class GameStarter : MonoBehaviour
{
    [SerializeField] private PlayDialogueSequence dialogueSequence;
    private void Start()
    {
        //dialogueSequence.StartPlaying();
    }
}
