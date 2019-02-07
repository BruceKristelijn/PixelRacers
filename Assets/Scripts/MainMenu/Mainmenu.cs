using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Mainmenu : MonoBehaviour
{

    public GameObject[] Menus;
    [Header("Other")]
    public GameObject Current;
    private GameObject CostumCurrent;
    public GameObject BackBtn;

    [Header("Managers")]
    public GameObject Carmanager;
    public GameObject Charmanager;

    [Header("Loading")]
    public GameObject loadingScreen;
    public Slider slider;
    public Text loadingText;

    [Header("Popup")]
    public GameObject Popup;
    public GameObject OK;
    public GameObject NO;
    public GameObject toptxt;
    public GameObject txt;
    public string OKstring;
    public int OKint;
    public Component OKcomponent;

    [Header("Costumization Screen")]
    public GameObject Cars;
    public GameObject Chars;
    public GameObject Taunts;

    public GameObject CarButton;
    public GameObject CharButton;

    [Header("Timetrial Screen")]
    public GameObject tt_parent;
    public Text tt_title;
    public Text tt_text;
    public Image tt_image;
    public Text tt_time;
    public int tt_trackId;
    public GameObject tt_button;

    [Header("Stats")]
    public Text Pixelcount;
    public Text Wincount;

    [Header("Settings")]
    public int volume;
    public GameObject Settings;
    public Slider Volume;
    public GameObject PlayBut;
    public GameObject GarageBut;
    public GameObject SetBut;
    public GameObject Credits;
    public GameObject Uppermenu;

    [Header("Uppermenu")]
    public GameObject pixels_mainmenu_ad;

    public GameObject ui_txt_playername_set;
    public GameObject ui_txt_token_set;

    public void ToScene(int MenuNmbr)
    {
        Current.SetActive(false);
        Current = Menus[MenuNmbr];
        Menus[MenuNmbr].SetActive(true);
    }

    //load this on start of the gae
    void Start()
    {
        PlayerPrefs.SetInt("PIXELCOUNT", 10000);


        AudioListener.volume = PlayerPrefs.GetFloat("SavedVolume", 1);
        Volume.value = AudioListener.volume;

        //Sync the username
        ui_txt_playername_set.GetComponent<Text>().text = PlayerPrefs.GetString("USERNAME");
        //sync the token for user feedback
        ui_txt_token_set.GetComponent<Text>().text = PlayerPrefs.GetString("TOKEN");


        //Get the current menu tab.
        for (int i = 0; i < Menus.Length; i++)
        {
            if (Menus[i].activeSelf)
            {
                Current = Menus[i];
                break;
            }
        }

        //Create the buttons for the car selection menu.
        for (int i = 0; i < GameObject.Find("CarManager").GetComponent<CarManager>().Cars.Length; i++)
        {
            GameObject Button = Instantiate(CarButton, Cars.transform);
            Button.GetComponent<ButtonInfo>().ButtonID = i;
        }

        //create the buttons for the character selection menu.
        for (int i = 0; i < GameObject.Find("CharManager").GetComponent<CharacterManager>().characters.Length; i++)
        {
            GameObject Button = Instantiate(CharButton, Chars.transform);
            Button.GetComponent<ButtonInfo>().ButtonID = i;
        }

        //Set wins to 0. Temporary
        PlayerPrefs.SetInt("WINCOUNT", 0);

        //Start the costmetic function to make the pixels "count up" on load.
        StartCoroutine(PixelCounter());
        //Do the same for the wins.
        StartCoroutine(WinCounter());

        //Generate a number between 1 and 5 so there is a 1/5 chance of getting an message tempting you to but pixels.
        var rng_pixelad = Random.Range(1, 5);
        Debug.Log(rng_pixelad);
        if (rng_pixelad == 3)
        {
            mainmenu_advertisepixels();
        }
    }

    //Scene load functions wich invokes the loading screen.
    public void loadScene(int sceneId)
    {
        StartCoroutine(LoadAsync(sceneId));
    }

    //Choosing function for the garage screen.
    public void SwitchChoosing(string SceneName)
    {
        if (CostumCurrent != null)
        {
            CostumCurrent.SetActive(false);
        }
        if (SceneName == "Cars")
        {
            CostumCurrent = Cars;
            Cars.SetActive(true);
        };
        if (SceneName == "Chars")
        {
            CostumCurrent = Chars;
            Chars.SetActive(true);
        };
        if (SceneName == "Taunts")
        {
            CostumCurrent = Taunts;
            Taunts.SetActive(true);
        };
    }

    //Function for opening the website from the settings menu
    public void VisitSite()
    {
        Application.OpenURL("http://www.savannadevelopments.com/");
    }

    //Dynamic popup functions, This to make sure users do the right things.
    public void CreatePopup(GameObject Calling, Component Called, string Title, string Text, int OKintf = 0, string Function = null)
    {
        if (!Popup.activeSelf)
        {
            Popup.SetActive(true);
        }
        if (Function == null)
        {
            NO.GetComponentInChildren<Text>().text = "OK";
            NO.GetComponent<RectTransform>().localPosition = new Vector2(0, this.transform.localPosition.y);
            if (OK.activeSelf)
            {
                OK.SetActive(false);
            }
        }
        else
        {
            if (!OK.activeSelf)
            {
                OK.SetActive(true);
            }
            NO.GetComponentInChildren<Text>().text = "NO";
            OK.GetComponent<RectTransform>().localPosition = new Vector2(120, this.transform.localPosition.y);
            NO.GetComponent<RectTransform>().localPosition = new Vector2(-120, this.transform.localPosition.y);
        }
        toptxt.GetComponent<Text>().text = Title;
        txt.GetComponent<Text>().text = Text;

        OKstring = Function;
        OKint = OKintf;
        OKcomponent = Called;
    }
    //Close the popup
    public void ClosePopup()
    {
        Popup.SetActive(false);
    }

    //Make the popup do something
    public void PopUpFunction()
    {
        switch (OKstring)
        {
            case "CarBuy":
                Carmanager.GetComponent<CarManager>().BuyCar(OKint);
                break;
            case "CharBuy":
                //Charmanager.GetComponent<CharacterManager> ().BuyChar(OKint);
                break;
            default:
                ClosePopup();
                break;
        }
    }

    //Loading function wich also invokes the loading screen.
    IEnumerator LoadAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId, LoadSceneMode.Single);

        if (Uppermenu.activeSelf) { Uppermenu.SetActive(false); }

        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            slider.value = progress;
            loadingText.text = (progress * 100).ToString("0") + "%";

            yield return null;
        }
    }

    //Load the settings menu
    public void Tosettings(int settings)
    {
        //if the int "settings" is 1 load the menu otherwise close it. This deceareses the amount of functions needed.
        if (settings == 1)
        {
            Settings.SetActive(true);
            PlayBut.SetActive(false);
            GarageBut.SetActive(false);
            Time.timeScale = 0;
        }
        else
        {
            Settings.SetActive(false);
            PlayBut.SetActive(true);
            GarageBut.SetActive(true);
            Time.timeScale = 1;
        }
    }

    //Change volume level
    public void Settings_Volume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("SavedVolume", volume);
    }
    //Credits function, This for starting a coroutine so the Unity UI buttons can open it.
    public void Rollcredits()
    {
        Time.timeScale = 0.7f;
        BeginFade(1);
        StartCoroutine(credits());
    }

    //--------------
    //Timetrial selection manager.
    //--------------

    //change the info on the track displayer
    public void tt_showinfo(string track_name, string track_info, Sprite track_image, int track_id, int track_sceneId)
    {
        //check if play button is active, if not set it to active.
        if (!tt_button.activeSelf)
        {
            tt_button.SetActive(true);
        }
        //Make the title the track name
        tt_title.text = track_name;
        //display the track info
        tt_text.text = track_info;
        //change the track image
        tt_image.sprite = track_image;
        //get the scene id
        tt_trackId = track_sceneId;

        //If the whole box isn't active set it to active
        if (!tt_parent.activeSelf)
        {
            tt_parent.SetActive(true);
        }

        //get an empty var
        var track_besttxt = "";

        //if there is a track time set get it to display
        if (PlayerPrefs.GetFloat("Ghost-Time-" + track_id, 6969) == 6969)
        {
            track_besttxt = "";
        }
        else
        {
            track_besttxt = "Best time: " + PlayerPrefs.GetFloat("Ghost-Time-" + track_id).ToString();
        }
        //display the string track_besttxt
        tt_time.text = track_besttxt;

    }

    public void tt_starttrack()
    {
        //if button is pressed load scene
        PlayerPrefs.SetInt("tt-chosentrack", tt_trackId);  //--HERE--
        StartCoroutine(LoadAsync(3));
    }

    //--------------
    //Cosmetic functions.
    //--------------

    IEnumerator credits()
    {
        yield return new WaitForSeconds(1);
        PlayBut.SetActive(false);
        GarageBut.SetActive(false);
        Uppermenu.SetActive(false);
        Credits.SetActive(true);
        Settings.SetActive(false);
        SetBut.SetActive(false);
        Credits.GetComponent<Animator>().SetBool("Start", true);
        yield return new WaitForSeconds(1);
        BeginFade(-1);
        yield return new WaitForSeconds(18);
        BeginFade(1);
        Time.timeScale = 1;
        yield return new WaitForSeconds(1);
        PlayBut.SetActive(true);
        GarageBut.SetActive(true);
        Uppermenu.SetActive(true);
        Credits.SetActive(false);
        Settings.SetActive(true);
        SetBut.SetActive(true);
        BeginFade(-1);
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0;
    }
    //Fade system.

    public Texture2D fadeOutTexture;    // the texture that will overlay the screen. This can be a black image or a loading graphic
    public float fadeSpeed = 1f;        // the fading speed

    private int drawDepth = -1000;      // the texture's order in the draw hierarchy: a low number means it renders on top
    private float alpha = 1.0f;         // the texture's alpha value between 0 and 1
    private int fadeDir = -1;           // the direction to fade: in = -1 or out = 1

    void OnGUI()
    {
        // fade out/in the alpha value using a direction, a speed and Time.deltaTime to convert the operation to seconds
        alpha += fadeDir * fadeSpeed * Time.deltaTime;
        // force (clamp) the number to be between 0 and 1 because GUI.color uses Alpha values between 0 and 1
        alpha = Mathf.Clamp01(alpha);

        // set color of our GUI (in this case our texture). All color values remain the same & the Alpha is set to the alpha variable
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth;                                                              // make the black texture render on top (drawn last)
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);       // draw the texture to fit the entire screen area
    }

    // sets fadeDir to the direction parameter making the scene fade in if -1 and out if 1
    public float BeginFade(int direction)
    {
        fadeDir = direction;
        return (fadeSpeed);
    }

    public IEnumerator PixelCounter()
    {
        for (int i = 0; i != (PlayerPrefs.GetInt("PIXELCOUNT")); i += 1)
        {
            Pixelcount.text = i.ToString();
            yield return new WaitForSeconds(0.005f);
        }
    }
    public IEnumerator WinCounter()
    {
        for (int i = 0; i < (PlayerPrefs.GetInt("WINCOUNT", 10) + 1); i++)
        {
            Wincount.text = i.ToString();
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void mainmenu_advertisepixels()
    {
        pixels_mainmenu_ad.SetActive(true);
    }



}

