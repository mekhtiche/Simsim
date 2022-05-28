using System;
using UnityEngine;
using UnityEngine.UI;
using ArabicSupport;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.Examples
{
	public class Speech_Rec : MonoBehaviour
	{
		private GCSpeechRecognition _speechRecognition;

		public Text _resultText;
		public Text Ref_word;
		public Image Mic;

		double dist = 0;

		private void Start()
		{
			_speechRecognition = GCSpeechRecognition.Instance;
			_speechRecognition.RecognizeSuccessEvent += RecognizeSuccessEventHandler;
			_speechRecognition.RecognizeFailedEvent += RecognizeFailedEventHandler;
			_speechRecognition.LongRunningRecognizeSuccessEvent += LongRunningRecognizeSuccessEventHandler;
			_speechRecognition.LongRunningRecognizeFailedEvent += LongRunningRecognizeFailedEventHandler;
			_speechRecognition.GetOperationSuccessEvent += GetOperationSuccessEventHandler;
			_speechRecognition.GetOperationFailedEvent += GetOperationFailedEventHandler;
			_speechRecognition.ListOperationsSuccessEvent += ListOperationsSuccessEventHandler;
			_speechRecognition.ListOperationsFailedEvent += ListOperationsFailedEventHandler;

			_speechRecognition.FinishedRecordEvent += FinishedRecordEventHandler;
			_speechRecognition.StartedRecordEvent += StartedRecordEventHandler;
			_speechRecognition.RecordFailedEvent += RecordFailedEventHandler;

			_speechRecognition.BeginTalkigEvent += BeginTalkigEventHandler;
			_speechRecognition.EndTalkigEvent += EndTalkigEventHandler;

			RefreshMicsButtonOnClickHandler();
			if (!_speechRecognition.HasConnectedMicrophoneDevices())
				return;
			_speechRecognition.SetMicrophoneDevice(_speechRecognition.GetMicrophoneDevices()[0]);
		}

		private void OnDestroy()
		{
			_speechRecognition.RecognizeSuccessEvent -= RecognizeSuccessEventHandler;
			_speechRecognition.RecognizeFailedEvent -= RecognizeFailedEventHandler;
			_speechRecognition.LongRunningRecognizeSuccessEvent -= LongRunningRecognizeSuccessEventHandler;
			_speechRecognition.LongRunningRecognizeFailedEvent -= LongRunningRecognizeFailedEventHandler;
			_speechRecognition.GetOperationSuccessEvent -= GetOperationSuccessEventHandler;
			_speechRecognition.GetOperationFailedEvent -= GetOperationFailedEventHandler;
			_speechRecognition.ListOperationsSuccessEvent -= ListOperationsSuccessEventHandler;
			_speechRecognition.ListOperationsFailedEvent -= ListOperationsFailedEventHandler;

			_speechRecognition.FinishedRecordEvent -= FinishedRecordEventHandler;
			_speechRecognition.StartedRecordEvent -= StartedRecordEventHandler;
			_speechRecognition.RecordFailedEvent -= RecordFailedEventHandler;

			_speechRecognition.EndTalkigEvent -= EndTalkigEventHandler;
		}

		private void Update()
		{

		}

		private void RefreshMicsButtonOnClickHandler()
		{
			_speechRecognition.RequestMicrophonePermission(null);
		}

		private void MicrophoneDevicesDropdownOnValueChangedEventHandler(int value)
		{
			if (!_speechRecognition.HasConnectedMicrophoneDevices())
				return;
			_speechRecognition.SetMicrophoneDevice(_speechRecognition.GetMicrophoneDevices()[value]);
		}

		public void StartRecordButtonOnClickHandler()
		{
			_resultText.text = string.Empty;
			_speechRecognition.StartRecord(false);
		}

		public void StopRecordButtonOnClickHandler()
		{
			_speechRecognition.StopRecord();
			_resultText.text = ArabicFixer.Fix("جاري التعرف على الكلام");
		}

		private void GetOperationButtonOnClickHandler()
		{
	
		}

		private void GetListOperationsButtonOnClickHandler()
		{
			// some parameters could be seted
			_speechRecognition.GetListOperations();
		}

		private void DetectThresholdButtonOnClickHandler()
		{
			_speechRecognition.DetectThreshold();
		}

		private void CancelAllRequetsButtonOnClickHandler()
		{
			_speechRecognition.CancelAllRequests();
		}

		private void RecognizeButtonOnClickHandler()
		{
			if (_speechRecognition.LastRecordedClip == null)
			{
				_resultText.text = "<color=red>No Record found</color>";
				return;
			}

			FinishedRecordEventHandler(_speechRecognition.LastRecordedClip, _speechRecognition.LastRecordedRaw);
		}

		private void StartedRecordEventHandler()
		{
			Mic.color = Color.red;
		}

		private void RecordFailedEventHandler()
		{
			_resultText.text = "<color=red>Start record Failed. Please check microphone device and try again.</color>";
		}

		private void BeginTalkigEventHandler()
		{
			_resultText.text = "<color=blue>Talk Began.</color>";
		}

		private void EndTalkigEventHandler(AudioClip clip, float[] raw)
		{
			_resultText.text += "\n<color=blue>Talk Ended.</color>";

			FinishedRecordEventHandler(clip, raw);
		}

		private void FinishedRecordEventHandler(AudioClip clip, float[] raw)
		{

			if (clip == null)
				return;

			RecognitionConfig config = RecognitionConfig.GetDefault();
			config.languageCode = (Enumerators.LanguageCode.ar_SA).Parse();
			config.audioChannelCount = clip.channels;
			// configure other parameters of the config if need

			GeneralRecognitionRequest recognitionRequest = new GeneralRecognitionRequest()
			{
				audio = new RecognitionAudioContent()
				{
					content = raw.ToBase64()
				},
				//audio = new RecognitionAudioUri() // for Google Cloud Storage object
				//{
				//	uri = "gs://bucketName/object_name"
				//},
				config = config
			};

			_speechRecognition.Recognize(recognitionRequest);
			Mic.color = Color.black;
		}

		private void GetOperationFailedEventHandler(string error)
		{
			_resultText.text = "Get Operation Failed: " + error;
		}

		private void ListOperationsFailedEventHandler(string error)
		{
			_resultText.text = "List Operations Failed: " + error;
		}

		private void RecognizeFailedEventHandler(string error)
		{
			_resultText.text = "Recognize Failed: " + error;
		}

		private void LongRunningRecognizeFailedEventHandler(string error)
		{
			_resultText.text = "Long Running Recognize Failed: " + error;
		}

		private void ListOperationsSuccessEventHandler(ListOperationsResponse operationsResponse)
		{
			_resultText.text = "List Operations Success.\n";

			if (operationsResponse.operations != null)
			{
				_resultText.text += "Operations:\n";

				foreach (var item in operationsResponse.operations)
				{
					_resultText.text += "name: " + item.name + "; done: " + item.done + "\n";
				}
			}
		}

		private void GetOperationSuccessEventHandler(Operation operation)
		{
			_resultText.text = "Get Operation Success.\n";
			_resultText.text += "name: " + operation.name + "; done: " + operation.done;

			if (operation.done && (operation.error == null || string.IsNullOrEmpty(operation.error.message)))
			{
				InsertRecognitionResponseInfo(operation.response);
			}
		}

		private void RecognizeSuccessEventHandler(RecognitionResponse recognitionResponse)
		{
			_resultText.text = "Recognize Success.";
			InsertRecognitionResponseInfo(recognitionResponse);
		}

		private void LongRunningRecognizeSuccessEventHandler(Operation operation)
		{
			if (operation.error != null || !string.IsNullOrEmpty(operation.error.message))
				return;

			_resultText.text = "Long Running Recognize Success.\n Operation name: " + operation.name;

			if (operation != null && operation.response != null && operation.response.results.Length > 0)
			{
				_resultText.text = "Long Running Recognize Success.";
				_resultText.text += "\n" + operation.response.results[0].alternatives[0].transcript;

				string other = "\nDetected alternatives:\n";

				foreach (var result in operation.response.results)
				{
					foreach (var alternative in result.alternatives)
					{
						if (operation.response.results[0].alternatives[0] != alternative)
						{
							other += alternative.transcript + ", ";
						}
					}
				}

				_resultText.text += other;
			}
			else
			{
				_resultText.text = "Long Running Recognize Success. Words not detected.";
			}
		}

		private void InsertRecognitionResponseInfo(RecognitionResponse recognitionResponse)
		{
			if (recognitionResponse == null || recognitionResponse.results.Length == 0)
			{
				_resultText.text = "\nWords not detected.";
				this.GetComponent<AudioSource>().PlayOneShot(this.GetComponent<Game_2>().clips[4]);
				return;
			}
			this.GetComponent<Game_2>().speech_result = recognitionResponse.results[0].alternatives[0].transcript;
			this.GetComponent<Game_2>().Check_result();
			/*			
						_resultText.text += "\n" + recognitionResponse.results[0].alternatives[0].transcript;

						var words = recognitionResponse.results[0].alternatives[0].words;

						if (words != null)
						{
							string times = string.Empty;

							foreach (var item in recognitionResponse.results[0].alternatives[0].words)
							{
								times += "<color=green>" + item.word + "</color> -  start: " + item.startTime + "; end: " + item.endTime + "\n";
							}

							_resultText.text += "\n" + times;
						}

						string other = "\nDetected alternatives: ";

						foreach (var result in recognitionResponse.results)
						{
							foreach (var alternative in result.alternatives)
							{
								if (recognitionResponse.results[0].alternatives[0] != alternative)
								{
									other += alternative.transcript + ", ";
								}
							}
						}

						_resultText.text += other;
						*/
		}
	}
}