using ReadSpeaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadSpeaker;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;


// This script should play a series of audio clips in order auomatically
// There should be a delay of 3 seconds between each clip.
// If the player presses the spacebar while the primary audio source is playing, the audio should pause and a interrupt audio clip should play.
// Once that interrupt audio clip is finished, the original audio should resume.
// If the player presses the spacebar during the 3 second delay, a message should be printed to console.
public class PlayDialogueSequence : MonoBehaviour
{
    public AudioSource primaryAudioSource;
    public AudioSource interruptAudioSource;
    public AudioSource yesAudioSource;
    public AudioSource yesExcitedAudioSource;
    public AudioSource yesQuestionAudioSource;
    public AudioClip[] clips;
    public string[] dialogueLines;
    public ResponseType[] correctResponseTypes;
    public ResponseType[] playerResponses;
    private int currentClipIndex = 0;
    public TTSSpeaker speaker;
    public TTSSpeaker interruptSpeaker;
    public AudioClip winAudio;
    public AudioClip loseClip;
    public AudioClip superWinClip;
    public Sprite normalgrandpa;
    public Sprite happygrandpa;
    public Sprite scowlinggrandpa;
    public TMP_Text subtitleText;
    public bool playing = false;
    //two tmp_text variables for the two different responses
    public TMP_Text excitedYesText;
    public TMP_Text questionYesText;
    //two int counters for the two different responses
    public int excitedYesCounter = 0;
    public int questionYesCounter = 0;
    public int correctResponses = 0;
    //tmptext for correct responses
    public TMP_Text correctResponsesText;
    public string[] randomInterruptResponses;
    public AudioClip[] randomInterruptClips;
    public bool useTTS = true;
    //canvas image for the grandpa
    public Image grandpaImage;
    //interrupt counter
    public int interruptCounter = 0;
    public int countForInterruptSlowdown = 5;
    public int slowdownamount = 10;
    public int slowdownRecovery = 2;
    public int standardSpeed = 100;


    
    
    
    // Start is called before the first frame update
    void Start()
    {
        TTS.Init();
        //playerResponses = new ResponseType[clips.Length];
        playerResponses = new ResponseType[dialogueLines.Length];
        speaker = GetComponent<TTSSpeaker>();
        grandpaImage.sprite = normalgrandpa;

    }
    private IEnumerator PlaySequence()
    {
        grandpaImage.sprite = normalgrandpa;
        questionYesCounter = 0;
        excitedYesCounter = 0;
        correctResponses = 0;
        currentClipIndex = 0;
        speaker.characteristics.Speed = standardSpeed;
        correctResponsesText.text = correctResponses.ToString();
        excitedYesText.text = excitedYesCounter.ToString();
        questionYesText.text = questionYesCounter.ToString();
        playing = true;
        while (currentClipIndex < dialogueLines.Length)
        //while (currentClipIndex < clips.Length)
        {
            speaker.characteristics.Speed += slowdownRecovery;
            if (speaker.characteristics.Speed > standardSpeed)
            {
                speaker.characteristics.Speed = standardSpeed;
            }



            //TTS.SayAsync(dialogueLines[currentClipIndex], speaker);
            grandpaImage.sprite = happygrandpa;
            if (useTTS)
            {
                TTS.Say(dialogueLines[currentClipIndex], speaker);
            } else
            {
                primaryAudioSource.clip = clips[currentClipIndex];
                primaryAudioSource.Play();
                yield return new WaitForSeconds(primaryAudioSource.clip.length);
            }


            subtitleText.text = dialogueLines[currentClipIndex];
            yield return new WaitForSeconds(0.5f);
            while (primaryAudioSource.isPlaying)
            {
                //if the player presses left or right arrow key, the primary audio source pauses and the interrupt audio source plays
                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    interruptCounter++;
                    grandpaImage.sprite = scowlinggrandpa;
                    
                    if (useTTS)
                    {
                        speaker.Pause();
                    } else
                    {
                        primaryAudioSource.Pause();
                    }

                    if (interruptCounter >= countForInterruptSlowdown)
                    {
                        speaker.characteristics.Speed -= slowdownamount;
                        interruptCounter = 0;
                        string responseText = "Since you keep interrupting me, I'll have to talk slower now.";
                        if (useTTS)
                        {
                            TTS.SayAsync(responseText, interruptSpeaker);
                        } else
                        {
                            interruptAudioSource.Play();
                        }
                        
                        subtitleText.text = responseText;
                    } else
                    {
                        int randomIndex = Random.Range(0, randomInterruptResponses.Length);
                        if (useTTS)
                        {
                            TTS.SayAsync(randomInterruptResponses[randomIndex], interruptSpeaker);
                        } else
                        {
                            interruptAudioSource.clip = randomInterruptClips[randomIndex];
                            interruptAudioSource.Play();
                        }

                        subtitleText.text = randomInterruptResponses[randomIndex];
                        
                    }
                    yield return new WaitForSeconds(1f);


                    //pick a random response from the array of responses

                    while (interruptAudioSource.isPlaying)
                    {
                        yield return null;
                    }
                    subtitleText.text = dialogueLines[currentClipIndex];
                    speaker.Resume();
                    grandpaImage.sprite = happygrandpa;
                    //interruptAudioSource.Play();
                    //yield return new WaitForSeconds(interruptAudioSource.clip.length);
                    //primaryAudioSource.UnPause();
                }
                yield return null;
            }


            //wait until the clip is done playing

            grandpaImage.sprite = normalgrandpa;

            bool responseGiven = false;
            while (responseGiven == false)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    yesExcitedAudioSource.Play();
                    playerResponses[currentClipIndex] = ResponseType.ExcitedYes;
                    responseGiven = true;
                    excitedYesCounter++;
                    excitedYesText.text = excitedYesCounter.ToString();
                }

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    yesQuestionAudioSource.Play();
                    playerResponses[currentClipIndex] = ResponseType.QuestionYes;
                    responseGiven = true;
                    questionYesCounter++;
                    questionYesText.text = questionYesCounter.ToString();
                }
                yield return null;
            }
            //if the player responds correctly, the curret responses variable increases
            if (playerResponses[currentClipIndex] == correctResponseTypes[currentClipIndex])
            {
                correctResponses++;
            }
            correctResponsesText.text = correctResponses.ToString();
            yield return new WaitForSeconds(1.5f);
            
            currentClipIndex++;


        }

        //if you get more than half of the responses correct, you win
        

        if (correctResponses >= correctResponseTypes.Length / 2)
        {
            //primaryAudioSource.clip = winAudio;
            //primaryAudioSource.Play();
            string response = "Oh thanks for listening, I don’t get many visitors anymore, nowadays I just look forward to your next game jam. Ha ha ha ha ha!";
            if (useTTS)
            {
                TTS.SayAsync(response, speaker);
            } else
            {
                primaryAudioSource.clip = winAudio;
                primaryAudioSource.Play();
            }
            subtitleText.text = response;
            grandpaImage.sprite = happygrandpa;
            Invoke("readyToPlayAgain", 10);

        }
        else if (correctResponses == correctResponseTypes.Length)
        {
            ////.clip = superWinClip;
            // primaryAudioSource.Play();
            string response = "It is I, the gamer god, and I congratulate you for answering all the questions correctly, you are a true gamer, now stop playing this game and touch grass.";
            if (useTTS)
            {
                TTS.SayAsync(response, speaker);
            } else
            {
                primaryAudioSource.clip = superWinClip;
                primaryAudioSource.Play();
            }
            grandpaImage.sprite = happygrandpa;
            subtitleText.text = response;
            Invoke("readyToPlayAgain", 10);
        }
        else
        {
            //primaryAudioSource.clip = loseClip;
            //primaryAudioSource.Play();
            string response = "Hey, hey stop mumbling in your sleep! God, now I have to repeat this whole thing to you.";
            if (useTTS)
            {
                TTS.SayAsync(response, speaker);
            } else
            {
                primaryAudioSource.clip = loseClip;
                primaryAudioSource.Play();
            }
            subtitleText.text = response;
            grandpaImage.sprite = scowlinggrandpa;
            //speaker.characteristics.Speed -= 10;
            currentClipIndex = 0;
            Invoke("RedoSequence", 10);
        }
        

    }

    private void readyToPlayAgain()
    {
        playing = false;
        subtitleText.text = "Left or right arrow to visit grandpa again.";
    }

    private void RedoSequence()
    {
        StartCoroutine(PlaySequence());
    }

    // Update is called once per frame
    void Update()
    {
        //if the player presses left or right arrow key start the sequence
        if (playing == false && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            StartCoroutine(PlaySequence());
        }

        //if the player presses the up arrow key, the speaker speed increases
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            speaker.characteristics.Speed += 10;
            yesExcitedAudioSource.pitch += .01f;
            yesQuestionAudioSource.pitch += .01f;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            speaker.characteristics.Speed -= 10;
            yesQuestionAudioSource.pitch -= .01f;
            yesExcitedAudioSource.pitch -= .01f;
        }
    }


}

public enum ResponseType
{
    ExcitedYes,
    QuestionYes
}

