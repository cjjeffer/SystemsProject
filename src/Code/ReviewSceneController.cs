using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Web.Helpers;
using Web.Twitter.API;
using Web.Twitter.DataStructures;
using UnityEngine.Video;

using UnityEngine.SceneManagement;

public class ReviewSceneController : MonoBehaviour
{

    public string TwitterApiConsumerKey = "UUFejwtHDNkeS85hxYw1JdtnV";
    public string TwitterApiConsumerSecret = "2CFNrhgtja55cYsDxkhGHqi9YQNBfNDGkBiFy41zn4G28bj4HK  ";

    public WebAccessToken TwitterApiAccessToken;

    public SendTextToAnalyse tempTweetAnalyzer;
    public int NumOfTweetsAnalyzed;
    public bool startAnalyzing;

    public String movieTitle = "Wonder Woman 1984";
    public Tweet[] SearchResults;
    int resultsIndex = -1;
    public int posCount = 0;
    public int negCount = 0;

    public float movieScore;
    string movieGrade;
    bool scoreCalculated;

    public MText.Modular3DText modular3DText;
    public VideoPlayer vid;
    bool vidStarted;
    public GameObject vidFinishedPackage;

    public TMPro.TextMeshProUGUI tweetDisplay;

    TheCrossOverSceneVars crosssOver;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("FailSafe", 10, 5);
        TwitterApiAccessToken = WebHelper.GetTwitterApiAccessToken(TwitterApiConsumerKey, TwitterApiConsumerSecret);
        
        if (tempTweetAnalyzer == null)
        {
            tempTweetAnalyzer = GameObject.FindGameObjectWithTag("Tweet Analyzer").GetComponent<SendTextToAnalyse>();
            
        }
        if (tempTweetAnalyzer != null)
        {
            tempTweetAnalyzer.RSController = this;
        }

        if(GameObject.FindGameObjectWithTag("GameController") != null)
        {
            crosssOver = GameObject.FindGameObjectWithTag("GameController").GetComponent<TheCrossOverSceneVars>();
            movieTitle = crosssOver.MovieToSearch;
        }

        modular3DText = GameObject.FindGameObjectWithTag("ReviewGradeText").GetComponent<MText.Modular3DText>();

        vid = GameObject.FindGameObjectWithTag("VideoPlayer").GetComponent<VideoPlayer>();

        ExampleFunction();
    }

    // Update is called once per frame
    void Update()
    {
        if (!vidStarted)
        {
            if (vid.isPlaying)
            {
                vidStarted = true;
            }
        }
        else
        {
            if (!vid.isPlaying)
            {
                DisplayScore();
            }
        }
        if (startAnalyzing)
        {
            tempTweetAnalyzer.SetTextToSend(SearchResults[NumOfTweetsAnalyzed].text);
            tempTweetAnalyzer.SendPredictionText();
            startAnalyzing = false;
        }

        if(NumOfTweetsAnalyzed >= (SearchResults.Length - 1) && !scoreCalculated && SearchResults.Length > 0)
        {
            CalculateMovieScore();
        }
    }

    public void AddPositive()
    {
        posCount++;
    }

    public void AddNegative()
    {
        negCount++;
    }

    public void IncrementTweetsAnalyzedCount()
    {
        NumOfTweetsAnalyzed++;
        if (NumOfTweetsAnalyzed < SearchResults.Length)
        {
            startAnalyzing = true;
        }
    }

    public async void ExampleFunction()
    {
        

        SearchResults = await TwitterRestApiHelper.SearchForTweets(movieTitle, this.TwitterApiAccessToken.access_token, 10);
        startAnalyzing = true;

    }

    void CalculateMovieScore()
    {
        movieScore = 100 - (100 * ((float)negCount / (float)SearchResults.Length));
        print("wheres the movie score? " + movieScore + " neg:"  + negCount + " search: " + SearchResults.Length);
        if(movieScore < 60)
        {
            movieGrade = "F";
        }

        else if(movieScore < 70 && movieScore >= 60)
        {
            movieGrade = "D";
        }

        else if (movieScore < 80 && movieScore >= 70)
        {
            movieGrade = "C";
        }

        else if (movieScore < 90 && movieScore >= 80)
        {
            movieGrade = "B";
        }

        else if (movieScore <= 99 && movieScore >= 90)
        {
            movieGrade = "A";
        }

        else if (movieScore == 100)
        {
            movieGrade = "A+";
        }

        scoreCalculated = true;
        modular3DText.Text = movieGrade;
    }

    public void nextTweet()
    {
        if(resultsIndex == SearchResults.Length - 1)
        {
            resultsIndex = -1;
            tweetDisplay.text = "Movie Score: " + movieScore + "% ;" + "Positive tweets: " + posCount + "; Negative tweets: " + negCount;

        }
        else
        {
            resultsIndex++;
            tweetDisplay.text = SearchResults[resultsIndex].user.screen_name + " : " + SearchResults[resultsIndex].text;
        }
    }

    public void prevTweet()
    {
        if(resultsIndex == 0)
        {
            resultsIndex = -1;
            tweetDisplay.text = "Movie Score: " + movieScore + "% ;" + "Positive tweets: " + posCount + "; Negative tweets: " + negCount;
        }
        else if(resultsIndex == -1) { 
        
                resultsIndex = SearchResults.Length - 1;
                tweetDisplay.text = SearchResults[resultsIndex].user.screen_name + " : " + SearchResults[resultsIndex].text;

        }
        else
        {
            resultsIndex--;
            tweetDisplay.text = SearchResults[resultsIndex].user.screen_name + " : " + SearchResults[resultsIndex].text;
        }
        
    }

    void DisplayScore()
    {
        vid.gameObject.SetActive(false);
        vidFinishedPackage.SetActive(true);
    }

    public void ReturnToMain()
    {
        SceneManager.LoadScene("Main Menu 1");
    }

    void FailSafe()
    {
        if(SearchResults.Length == 0)
        {

            TwitterApiAccessToken = WebHelper.GetTwitterApiAccessToken(TwitterApiConsumerKey, TwitterApiConsumerSecret);
            ExampleFunction();
        }
    }
}
