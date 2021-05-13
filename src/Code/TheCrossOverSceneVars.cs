using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TheCrossOverSceneVars : MonoBehaviour
{
    public static TheCrossOverSceneVars _instance;
    public List<string> sceneNames;
    public string MovieToSearch;
    public MText.Mtext_UI_InputField userInputField;
    public static TheCrossOverSceneVars instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<TheCrossOverSceneVars>();
            return _instance;
        }
    }

    private void Start()
    {
        //if(userInputField == null)
        //{
        //    userInputField = GameObject.FindGameObjectWithTag("Input Field").GetComponent<MText.Mtext_UI_InputField>();
        //}
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu" || SceneManager.GetActiveScene().name == "Main Menu 1")
        {
            if (userInputField == null)
            {
                userInputField = GameObject.FindGameObjectWithTag("Input Field").GetComponent<MText.Mtext_UI_InputField>();
            }

            MovieToSearch = userInputField.Text;
            if (Input.GetKeyDown(KeyCode.Return))
            {
                print("enter triggered");
                int indexChosen = Random.Range(0, sceneNames.Count - 1);
                SceneManager.LoadScene(sceneNames[indexChosen]);
            }

            
        }
    }

    public string GetMovieToSearch()
    {
        return MovieToSearch;
    }
}
