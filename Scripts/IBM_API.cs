using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using IBM.Watson.DeveloperCloud.Services.NaturalLanguageUnderstanding.v1;
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Connection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
//using IBM.Watson.DeveloperCloud.DataTypes;
//using IBM.Watson.DeveloperCloud.Debug;
//using IBM.Watson.DeveloperCloud.Editor;
//using IBM.Watson.DeveloperCloud.Logging;
//using IBM.Watson.DeveloperCloud.Services;
//using IBM.Watson.DeveloperCloud.UnitTests;
//using IBM.Watson.DeveloperCloud.Utilities;
//using IBM.Watson.DeveloperCloud.Widgets;

public class IBM_API : MonoBehaviour {
	string story;
	string targetPath;
	string[] lines;
	int audioClipCounter;
	int synthChunksSize;
	bool gatheringAudio;
	bool loadingChunk;
	List<string> customFoundWords;
	AudioClip[] narrationClips;

	StoryData storyData;
	NaturalLanguageUnderstanding _naturalLanguageUnderstanding;
	TextToSpeech _textToSpeech;

	void Start () {
		customFoundWords = new List<string>();
		storyData = GetComponent<StoryData> ();

		Credentials analyze_cred = new Credentials("dfb6ed8f-ca63-4c3f-8723-95c92aebaa83",
			"bbNFyUD1LMDg", "https://gateway.watsonplatform.net/natural-language-understanding/api");
		_naturalLanguageUnderstanding = new NaturalLanguageUnderstanding(analyze_cred);

		Credentials textSpeech_cred = new Credentials("4f7ef3c9-414a-463e-bd94-200da766011b",
			"QVPX0Zz66Dbb", "https://stream.watsonplatform.net/text-to-speech/api");
		_textToSpeech = new TextToSpeech(textSpeech_cred);
	}

	public void getKeywords() {;
//		lines = GetComponent<Google_API> ().lines;
		lines = System.IO.File.ReadAllLines(GetComponent<Google_API>().targetPath);
		StartCoroutine(Analyze());
	}

	IEnumerator Analyze()
	{
		Debug.Log ("analyzing story");
		List<string> apiChunks = GetChunksFromStory ();
		loadingChunk = true;

		foreach (string chunk in apiChunks)
		{
			//Analyze chunk
			Parameters p = GetAnalyzeParams(chunk);
			customAnalyze (chunk);
			_naturalLanguageUnderstanding.Analyze (OnAnalyze, OnFail, p);
			yield return new WaitWhile (()=> loadingChunk);
			loadingChunk = true;
		}
		Debug.Log ("Story analyzed!");
		Debug.Log ("Adding debug text");
		GetDebugText ();
        startNarration();
	}

	private void customAnalyze(string text) {
		text = text.ToLower ();
		if (text.Contains ("morning")) {
			customFoundWords.Add ("morning");
		} else if (text.Contains ("dawn")) {
			customFoundWords.Add ("dawn");
		} else if (text.Contains ("afternoon")) {
			customFoundWords.Add ("afternoon");
		} else if (text.Contains ("noon")) {
			customFoundWords.Add ("noon");
		} else if (text.Contains ("moon")) {
			customFoundWords.Add ("night");
		}
		else if (text.Contains ("dusk")) {
			customFoundWords.Add ("dusk");
		} else if (text.Contains("night")) {
			customFoundWords.Add ("night");
		} else if (text.Contains("midnight")) {
			customFoundWords.Add ("midnight");
		}
	}

	private void OnAnalyze(AnalysisResults resp, Dictionary<string, object> customData)
	{
		List<string> simplifiedKeywords = new List<string> ();
		string json = customData ["json"].ToString ();
		List<Keyword> keyWords = JsonConvert.DeserializeObject<RootObject>(json).keywords;
		List<Entity> entities = JsonConvert.DeserializeObject<RootObject>(json).entities;

		foreach (Keyword k in keyWords) {
			simplifiedKeywords.Add (k.text);
		}
		foreach (string s in customFoundWords) {
			simplifiedKeywords.Add (s);
		}
		storyData.addParagraph (simplifiedKeywords, entities);
		loadingChunk = false;
	}
		
	Parameters GetAnalyzeParams(string text) {
		Parameters p = new Parameters ();
		p.text = text;

		Features f = new Features ();
		EntitiesOptions e = new EntitiesOptions ();
		e.emotion = true;
		e.sentiment = false;
		e.limit = 2;
		KeywordsOptions k = new KeywordsOptions ();
		k.emotion = true;
		k.sentiment = false;
		k.limit = 13;
		f.entities = e;
		f.keywords = k;
		p.features = f;
		return p;
	}

	public void startNarration() {
		StartCoroutine (Synthesize ());
	}

	IEnumerator Synthesize()
	{
		int counter = 0;
		List<string> apiChunks = GetChunksFromStory ();
		_textToSpeech.Voice = VoiceType.en_US_Allison;
		synthChunksSize = apiChunks.Count;
		narrationClips = new AudioClip[synthChunksSize];
		gatheringAudio = true;
		loadingChunk = true;

		Debug.Log ("Gathering audio (this can take 30 seconds)");
		foreach (string chunk in apiChunks)
		{
			_textToSpeech.ToSpeech(OnSynthesize, OnFail, chunk, true);
			yield return new WaitWhile (()=> loadingChunk);
			loadingChunk = true;
		}
		storyData.addAudioClips (narrationClips);
		Debug.Log ("Finished gathering story");
		storyData.doneLoading ();
	}

	private void OnSynthesize(AudioClip clip, Dictionary<string, object> customData)
	{
		narrationClips [audioClipCounter] = clip;
		audioClipCounter++;
		loadingChunk = false;

		if (audioClipCounter == synthChunksSize)
			gatheringAudio = false;
	}

	private void GetDebugText() {
		List<string> apiChunks = GetChunksFromStory ();
		string[] chunkArray = new string[apiChunks.Count];
		int count = 0;
		foreach (string chunk in apiChunks) {
			chunkArray [count] = chunk;
			count++;
		}
		storyData.addDebugText (chunkArray);
	}

	//Get linesPerChunk lines at a time so the api can send
	//multiple lines from the story at once
	private List<string> GetChunksFromStory() {
		int linesPerChunk = 7;
		int counter = 0;
		string chunk = "";
		List<string> list = new List<string> ();
		foreach (string line in lines) {
			if (counter < linesPerChunk - 1) {
				chunk += line;
				counter++;
				continue;
			}
			chunk += line;
			list.Add (chunk);
			counter = 0;
			chunk = "";
		}
		if (chunk != null && chunk.Length != 0) {
			list.Add (chunk);
		}
		return list;
	}

	private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
	{
		Debug.Log("IBM Watson api call failed");
		Debug.Log (error.ErrorMessage);
	}
}

[System.Serializable]
public class Usage
{
	public int text_units;
	public int text_characters;
	public int features;
}

[System.Serializable]
public class Sentiment
{
	public double score;
	public string label;
}

[System.Serializable]
public class Emotion
{
	public double sadness;
	public double joy;
	public double fear;
	public double disgust;
	public double anger;
}

[System.Serializable]
public class Keyword
{
	public string text;
	public Sentiment sentiment;
	public double relevance;
	public Emotion emotion;
}

[System.Serializable]
public class Disambiguation
{
	public List<string> subtype;
	public string name;
	public string dbpedia_resource;
}

[System.Serializable]
public class Entity
{
	public string type;
	public string text;
	public Sentiment sentiment; //was 2
	public double relevance;
	public Emotion emotion; //was 2
	public Disambiguation disambiguation;
	public int count;
}

[System.Serializable]
public class RootObject
{
	public Usage usage;
	public string language;
	public List<Keyword> keywords;
	public List<Entity> entities;
}