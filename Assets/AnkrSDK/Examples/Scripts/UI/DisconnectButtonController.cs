#if !UNITY_WEBGL || UNITY_EDITOR
using AnkrSDK.WalletConnectSharp.Core;
using AnkrSDK.WalletConnectSharp.Unity;
#endif
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.UI
{
	public class DisconnectButtonController : MonoBehaviour
	{
		[SerializeField] private Button _button;

	#if !UNITY_WEBGL || UNITY_EDITOR
		private void OnEnable()
		{
			SubscribeOnTransportEvents().Forget();

			_button.onClick.AddListener(OnButtonClick);
		}

		private void OnDisable()
		{
			UnsubscribeFromTransportEvents();

			_button.onClick.RemoveAllListeners();
		}

		private async UniTaskVoid SubscribeOnTransportEvents()
		{
			if (WalletConnect.Instance == null)
			{
				await UniTask.WaitWhile(() => WalletConnect.Instance == null);
			}

			if (WalletConnect.ActiveSession == null)
			{
				return;
			}

			WalletConnect.ActiveSession.OnTransportConnect += UpdateDisconnectButtonState;
			WalletConnect.ActiveSession.OnTransportDisconnect += UpdateDisconnectButtonState;
			WalletConnect.ActiveSession.OnTransportOpen += UpdateDisconnectButtonState;
		}

		private void UnsubscribeFromTransportEvents()
		{
			if (WalletConnect.ActiveSession == null)
			{
				return;
			}

			WalletConnect.ActiveSession.OnTransportConnect -= UpdateDisconnectButtonState;
			WalletConnect.ActiveSession.OnTransportDisconnect -= UpdateDisconnectButtonState;
			WalletConnect.ActiveSession.OnTransportOpen -= UpdateDisconnectButtonState;
		}

		private void UpdateDisconnectButtonState(object sender, WalletConnectProtocol e)
		{
			_button.gameObject.SetActive(!e.Disconnected);
		}

		private static void OnButtonClick()
		{
			WalletConnect.CloseSession().Forget();
		}
	#else
		private void Awake()
		{
			gameObject.SetActive(false);
		}
	#endif
	}
}