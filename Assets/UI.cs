using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Syncano;
using Syncano.Data;

public class UI : MonoBehaviour {

	public RectTransform PanelCreateAccount;
	public RectTransform PanelUserProfile;
	public RectTransform PanelLogin;
	public RectTransform PanelMainMenu;

	public Image AvatarImage;
	public Image AvatarImageDetails;

	public InputField InputFieldNickname;
	public InputField InputFieldCountry;

	public Text NumberOfTrophies1;
	public Text NumberOfTrophies2;
	public Text NumberOfTrophies3;

	public Text NumberOfWins;
	public Text NumberOfDefeats;
	public Text NumberOfDraws;

	public Text TextSummary;
	public Text TextUsername;
	public Text TextPassword;

	public Text TextUsernameLogin;
	public Text TextUsernamePassword;

	public Button SubmitButton;
	public Button SignUpButton;
	public Button UpdateButton;

	private List<RectTransform> allPanels = new List<RectTransform>();
	private List<Text> allText = new List<Text>();

	private User<TestProfile> user;

	void Start()
	{
		allPanels.Add(PanelCreateAccount);
		allPanels.Add(PanelUserProfile);
		allPanels.Add(PanelLogin);
		allPanels.Add(PanelMainMenu);

		allText.Add(TextSummary);
		allText.Add(TextUsername);
		allText.Add(TextPassword);
		allText.Add(TextUsernameLogin);
		allText.Add(TextUsernamePassword);

		SyncanoClient.Instance.Init("YOUR_API_KEY", "YOUR_INSTANCE_NAME");
	}

	public void OnLoginButtonClicked()
	{
		DisablePanels();
		PanelLogin.gameObject.SetActive(true);
	}

	public void OnCreateAccountButtonClicked()
	{
		DisablePanels();
		PanelCreateAccount.gameObject.SetActive(true);
	}

	public void OnSelectAvatarButtonClicked()
	{
		OpenFile(AvatarImage);
	}

	public void OnHomeButtonPressed()
	{
		DisablePanels();
		PanelMainMenu.gameObject.SetActive(true);
	}

	public void OnSubmitButtonClicked()
	{
		if(string.IsNullOrEmpty(TextUsername.text))
		{
			SetSummaryText("Please enter Username", Color.red);
			return;
		}

		else if(string.IsNullOrEmpty(TextPassword.text))
		{
			SetSummaryText("Please enter Password", Color.red);
			return;
		}

		else if(AvatarImage.sprite == null)
		{
			SetSummaryText("Please Select Avatar", Color.red);
			return;
		}

		StartCoroutine(CreateNewProfile());
	}

	public void OnSignInButtonClicked()
	{
		if(string.IsNullOrEmpty(TextUsernameLogin.text))
		{
			SetSummaryText("Please enter Username", Color.red);
			return;
		}

		else if(string.IsNullOrEmpty(TextUsernamePassword.text))
		{
			SetSummaryText("Please enter Password", Color.red);
			return;
		}

		StartCoroutine(SignIn());
	}

	public void OnUpdateButtonClicked()
	{
		StartCoroutine(UpdateProfile());
	}

	public void OnAvatarChange()
	{
		OpenFile(AvatarImageDetails);
	}

	private IEnumerator UpdateProfile()
	{
		SetSummaryText("updating...", Color.white);

		user.UserName = InputFieldNickname.text;
		user.Profile.Country = InputFieldCountry.text;
		user.Profile.Avatar = new SyncanoFile(AvatarImageDetails.sprite.texture.EncodeToPNG());

		yield return user.UpdateCustomUser(OnUserUpdateSuccess, OnUserUpdateFail);
	}

	private void SetSummaryText(string text, Color textColor = default(Color), int fontSize = 50)
	{
		TextSummary.text = text;
		TextSummary.fontSize = fontSize;
		TextSummary.color = textColor;
	}

	private void DisablePanels()
	{
		foreach(RectTransform panel in allPanels)
		{
			panel.gameObject.SetActive(false);
		}

		foreach(Text t in allText)
		{
			t.text = string.Empty;
		}
	}

	private IEnumerator CreateNewProfile()
	{
		SetSummaryText("Sending...", Color.white);
		user = new User<TestProfile>(TextUsername.text, TextPassword.text);

		int randomTrophies1 = Random.Range(0, 5);
		int randomTrophies2 = Random.Range(0, 6);
		int randomTrohpies3 = Random.Range(0, 8);
		int randomVictories = Random.Range(0, 3);
		int randomDefeats = Random.Range(0, 3);
		string country = "EN";
		SyncanoFile avatar = new SyncanoFile(AvatarImage.sprite.texture.EncodeToPNG());

		user.Profile = new TestProfile(randomVictories, randomDefeats, country, avatar, randomTrophies1, randomTrophies2, randomTrophies2);

		SubmitButton.enabled = false;
		yield return user.Register(OnUserRegisterSuccess, OnUserRegisterFail);
	}

	private IEnumerator SignIn()
	{
		SetSummaryText("Connecting...", Color.white);
		user = new User<TestProfile>(TextUsernameLogin.text, TextUsernamePassword.text);
		SignUpButton.enabled = false;
		yield return user.Login(OnUserSigninSuccess, OnUserSigninFail);
	}

	private void OnUserRegisterSuccess(Response<User<TestProfile>> response)
	{
		SubmitButton.enabled = true;
		SetSummaryText("success", Color.green);

	}

	private void OnUserRegisterFail(Response<User<TestProfile>> response)
	{
		SubmitButton.enabled = true;

		if(response.IsSyncanoError)
		{
			SetSummaryText(response.syncanoError, Color.red, 30);
		}

		else
		{
			SetSummaryText(response.webError, Color.red, 30);
		}
	}

	private void OnUserSigninSuccess(Response<User<TestProfile>> response)
	{
		SignUpButton.enabled = true;
		SetSummaryText("success, please wait...", Color.green);
		DisablePanels();
		PanelUserProfile.gameObject.SetActive(true);

		StartCoroutine(PrepareUserProfilePanel());
	}

	private void OnUserSigninFail(Response<User<TestProfile>> response)
	{
		SignUpButton.enabled = true;

		if(response.IsSyncanoError)
		{
			SetSummaryText(response.syncanoError, Color.red, 30);
		}

		else
		{
			SetSummaryText(response.webError, Color.red, 30);
		}
	}

	private void OnUserUpdateSuccess(Response<User<TestProfile>> response)
	{
		UpdateButton.enabled = true;
		SetSummaryText("updated!", Color.green);
	}

	private void OnUserUpdateFail(Response<User<TestProfile>> response)
	{
		UpdateButton.enabled = true;

		if(response.IsSyncanoError)
		{
			SetSummaryText(response.syncanoError, Color.red, 30);
		}

		else
		{
			SetSummaryText(response.webError, Color.red, 30);
		}
	}

	private void OpenFile(Image image)
	{
		Texture2D texture = new Texture2D(0, 0);

		if( texture == null )
		{
			EditorUtility.DisplayDialog( "Select Texture", "You must select a texture first!", "OK" );
			return;
		}

		string path = EditorUtility.OpenFilePanel( "Select Avatar", "Assets/Resources/", "png" );
		if( path.Length != 0 )
		{
			WWW www = new WWW( "file:///" + path );
			www.LoadImageIntoTexture(texture);

			Sprite s = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
			image.sprite = s;
		}
	}

	private IEnumerator PrepareUserProfilePanel()
	{
		SetSummaryText("Loading...", Color.white);

		Texture2D texture = new Texture2D(0, 0);
		WWW www = new WWW(user.Profile.Avatar.Value);
		yield return www;

		www.LoadImageIntoTexture(texture);
		Sprite s = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

		AvatarImageDetails.sprite = s;

		InputFieldNickname.text = user.UserName;
		InputFieldCountry.text = user.Profile.Country;

		NumberOfTrophies1.text = user.Profile.GoldenTrophies.ToString();
		NumberOfTrophies2.text = user.Profile.SilverTrophies.ToString();
		NumberOfTrophies3.text = user.Profile.BronzeTrophies.ToString();

		NumberOfWins.text = user.Profile.Victories.ToString();
		NumberOfDefeats.text = user.Profile.Defeats.ToString();
		NumberOfDraws.text = user.Profile.Draws.ToString();

		SetSummaryText("Profile", Color.white);

		yield return null;
	}
}
