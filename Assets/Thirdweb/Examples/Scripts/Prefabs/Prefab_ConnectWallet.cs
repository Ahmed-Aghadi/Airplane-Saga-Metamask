using System.Collections.Generic;
using UnityEngine;
using Thirdweb;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

using System.Threading.Tasks;

using MetaMask.Unity;
using MetaMask.Models;

using Nethereum.Hex.HexTypes;

public enum Wallet
{
    MetaMask,
    Injected,
    CoinbaseWallet,
    WalletConnect,
    MagicAuth,
    MetamaskMobile
}

[Serializable]
public struct WalletButton
{
    public Wallet wallet;
    public GameObject walletButton;
    public Sprite icon;
}

[Serializable]
public struct NetworkSprite
{
    public Chain chain;
    public Sprite sprite;
}


public class GetBalanceParams
{
    [JsonProperty("address")]
    [JsonPropertyName("address")]
    public string Address { get; set; }

}
public class SwitchChainParams
{

    [JsonProperty("chainId")]
    [JsonPropertyName("chainId")]
    public string ChainId { get; set; }

}

public class AddEthereumChainParams
{
    [JsonProperty("chainId")]
    [JsonPropertyName("chainId")]
    public string ChainId { get; set; }

    [JsonProperty("chainName")]
    [JsonPropertyName("chainName")]
    public string ChainName { get; set; }

    [JsonProperty("rpcUrls")]
    [JsonPropertyName("rpcUrls")]
    public string[] RpcUrls { get; set; }

    [JsonProperty("nativeCurrency")]
    [JsonPropertyName("nativeCurrency")]
    public NativeCurrencyCustom NativeCurrency { get; set; }

    [JsonProperty("blockExplorerUrls")]
    [JsonPropertyName("blockExplorerUrls")]
    public string[] BlockExplorerUrls { get; set; }
}
public class NativeCurrencyCustom
{
    [JsonProperty("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonProperty("symbol")]
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; }

    [JsonProperty("decimals")]
    [JsonPropertyName("decimals")]
    public int Decimals { get; set; }
}

public class ErrorJson
{
    [JsonProperty("code")]
    [JsonPropertyName("code")]
    public string code { get; set; }

}

public class Prefab_ConnectWallet : MonoBehaviour
{

    [Header("Metamask SDK")]
    public Button connectWalletButton;
    public Sprite connectWalletButtonIcon;

    [Header("SETTINGS")]
    public List<Wallet> supportedWallets;
    public bool supportSwitchingNetwork;

    [Header("CUSTOM CALLBACKS")]
    public UnityEvent OnConnectedCallback;
    public UnityEvent OnDisconnectedCallback;
    public UnityEvent OnSwitchNetworkCallback;

    [Header("UI ELEMENTS (DO NOT EDIT)")]
    // Connecting
    public GameObject connectButton;
    public GameObject connectDropdown;
    public List<WalletButton> walletButtons;
    // Connected
    public GameObject connectedButton;
    public GameObject connectedDropdown;
    public TMP_Text balanceText;
    public TMP_Text walletAddressText;
    public Image walletImage;
    public TMP_Text currentNetworkText;
    public Image currentNetworkImage;
    public Image chainImage;
    // Network Switching
    public GameObject networkSwitchButton;
    public GameObject networkDropdown;
    public GameObject networkButtonPrefab;
    public List<NetworkSprite> networkSprites;

    string address;
    Wallet wallet;


    private void Awake()
    {
        MetaMaskUnity.Instance.Initialize();
        var wallet = MetaMaskUnity.Instance.Wallet;
        wallet.AccountChanged += OnAccountChanged;
        wallet.ChainIdChanged += OnChainIdChanged;
        MetaMaskUnity.Instance.LoadSession();
    }


    // UI Initialization

    private void Start()
    {
        address = null;

        if (supportedWallets.Count == 1)
            connectButton.GetComponent<Button>().onClick.AddListener(() => OnConnect(supportedWallets[0]));
        else
            connectButton.GetComponent<Button>().onClick.AddListener(() => OnClickDropdown());

        connectWalletButton.onClick.AddListener(() => OnConnect(Wallet.MetamaskMobile));
        foreach (WalletButton wb in walletButtons)
        {
            if (supportedWallets.Contains(wb.wallet))
            {
                wb.walletButton.SetActive(true);
                wb.walletButton.GetComponent<Button>().onClick.AddListener(() => OnConnect(wb.wallet));
            }
            else
            {
                wb.walletButton.SetActive(false);
            }
        }

        connectButton.SetActive(true);
        connectedButton.SetActive(false);

        connectDropdown.SetActive(false);
        connectedDropdown.SetActive(false);

        networkSwitchButton.SetActive(supportSwitchingNetwork);
        networkDropdown.SetActive(false);
    }

    public void MetamaskConnect()
    {
        print("Connect clicked");
        var wallet = MetaMaskUnity.Instance.Wallet;
        wallet.Connect();

    }
    public void OnAccountChanged(object sender, EventArgs e)
    {
        print("Account Changed");
        print("Wallet address OnAccountChanged: " + MetaMaskUnity.Instance.Wallet.SelectedAddress);
        try
        {
            address = MetaMaskUnity.Instance.Wallet.SelectedAddress;
            var chainHex = ThirdwebManager.Instance.chain == Chain.MantleTestnet ? "0x1389" : "0x12";
            if (MetaMaskUnity.Instance.Wallet.SelectedChainId != chainHex)
            {
                print("Chain ID Metamask: " + MetaMaskUnity.Instance.Wallet.SelectedChainId);
                print("Chain ID Required: " + ThirdwebManager.Instance.chain);
                OnSwitchNetwork(ThirdwebManager.Instance.chain);
            }
            else
            {
                OnConnected();
            }
            PlayMakerGlobals.Instance.Variables.FindFsmBool("IsWalletConnected").Value = true;
            if (OnConnectedCallback != null)
                OnConnectedCallback.Invoke();
            print($"Connected successfully to: {address}");
        }
        catch (Exception ex)
        {
            print($"Error Connecting Wallet: {ex.Message}");
        }
    }

    public async Task<string> GetBalance(string walletAddress)
    {
        var paramsArray = new string[] { walletAddress };
        var request = new MetaMaskEthereumRequest
        {
            Method = "eth_getBalance",
            Parameters = paramsArray
        };
        // onTransactionSent?.Invoke(this, EventArgs.Empty);
        var response = await MetaMaskUnity.Instance.Wallet.Request(request);
        print("response: " + response.ToString());
        return response.ToString();
    }

    public void ConnectAgain()
    {
        Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
        if (wallet == Wallet.MetamaskMobile)
        {
            MetaMaskUnity.Instance.LoadSession();
        }
        OnConnect(wallet);
    }

    // Connecting

    public async void OnConnect(Wallet _wallet)
    {
        try
        {
            wallet = _wallet;
            PlayMakerGlobals.Instance.Variables.FindFsmInt("WALLET").Value = (int)_wallet;
            PlayMakerGlobals.Instance.Variables.FindFsmInt("ChainId").Value = (int)ThirdwebManager.Instance.chain;
            if (_wallet == Wallet.MetamaskMobile)
            {
                print("Metamask Button clicked");
                PlayMakerGlobals.Instance.Variables.FindFsmBool("IsMetamaskClicked").Value = true;
                PlayMakerGlobals.Instance.Variables.FindFsmBool("HideQrCode").Value = false;
                MetaMaskUnity.Instance.Wallet.Connect();
            }
            else
            {
                print("Other Buttons clicked");
                PlayMakerGlobals.Instance.Variables.FindFsmBool("HideQrCode").Value = true;
                address = await ThirdwebManager.Instance.SDK.wallet.Connect(
                   new WalletConnection()
                   {
                       provider = GetWalletProvider(_wallet),
                       chainId = (int)ThirdwebManager.Instance.chain,
                   });

                OnConnected();
                PlayMakerGlobals.Instance.Variables.FindFsmBool("IsWalletConnected").Value = true;
                if (OnConnectedCallback != null)
                    OnConnectedCallback.Invoke();
                print($"Connected successfully to: {address}");
            }
        }
        catch (Exception e)
        {
            print($"Error Connecting Wallet: {e.Message}");
        }
    }

    async void OnConnected()
    {
        try
        {
            MetaMaskUnity.Instance.SaveSession();
            Chain _chain = ThirdwebManager.Instance.chain;
            PlayMakerGlobals.Instance.Variables.FindFsmInt("ChainId").Value = (int)_chain;
            if (wallet == Wallet.MetamaskMobile)
            {
                print("address: " + MetaMaskUnity.Instance.Wallet.SelectedAddress);
                print("chain: " + MetaMaskUnity.Instance.Wallet.SelectedChainId);
                string balanceHex = await GetBalance(address);
                var balanceHexBigInt = new HexBigInteger(balanceHex);
                var balanceBigInt = balanceHexBigInt.Value;
                print("balance parsed: " + balanceBigInt.ToString());
                if (_chain == Chain.ThunderCoreTestnet)
                {
                    balanceText.text = $"{balanceBigInt.ToString().ToEth()} TST";
                }
                else if (_chain == Chain.MantleTestnet)
                {
                    balanceText.text = $"{balanceBigInt.ToString().ToEth()} BIT";
                }
                else
                {
                    balanceText.text = $"{balanceBigInt.ToString().ToEth()} ETH";
                }
                walletAddressText.text = address.ShortenAddress();
                currentNetworkText.text = ThirdwebManager.Instance.chainIdentifiers[_chain];
                currentNetworkImage.sprite = networkSprites.Find(x => x.chain == _chain).sprite;
                connectButton.SetActive(false);
                connectedButton.SetActive(true);
                connectDropdown.SetActive(false);
                connectedDropdown.SetActive(false);
                networkDropdown.SetActive(false);
                walletImage.sprite = connectWalletButtonIcon;
                chainImage.sprite = networkSprites.Find(x => x.chain == _chain).sprite;
            }
            else
            {
                CurrencyValue nativeBalance = await ThirdwebManager.Instance.SDK.wallet.GetBalance();
                if (_chain == Chain.ThunderCoreTestnet)
                {
                    balanceText.text = $"{nativeBalance.value.ToEth()} TST";
                }
                else if (_chain == Chain.MantleTestnet)
                {
                    balanceText.text = $"{nativeBalance.value.ToEth()} BIT";
                }
                else
                {
                    balanceText.text = $"{nativeBalance.value.ToEth()} {nativeBalance.symbol}";
                }
                walletAddressText.text = address.ShortenAddress();
                currentNetworkText.text = ThirdwebManager.Instance.chainIdentifiers[_chain];
                currentNetworkImage.sprite = networkSprites.Find(x => x.chain == _chain).sprite;
                connectButton.SetActive(false);
                connectedButton.SetActive(true);
                connectDropdown.SetActive(false);
                connectedDropdown.SetActive(false);
                networkDropdown.SetActive(false);
                walletImage.sprite = walletButtons.Find(x => x.wallet == wallet).icon;
                chainImage.sprite = networkSprites.Find(x => x.chain == _chain).sprite;
            }
        }
        catch (Exception e)
        {
            print($"Error Fetching Native Balance: {e.Message}");
        }

    }

    // Disconnecting

    public async void OnDisconnect()
    {
        try
        {
            if (wallet == Wallet.MetamaskMobile)
            {
                MetaMaskUnity.Instance.Disconnect();
            }
            else
            {
                await ThirdwebManager.Instance.SDK.wallet.Disconnect();
            }
            OnDisconnected();
            PlayMakerGlobals.Instance.Variables.FindFsmBool("IsWalletConnected").Value = false;
            if (OnDisconnectedCallback != null)
                OnDisconnectedCallback.Invoke();
            print($"Disconnected successfully.");

        }
        catch (Exception e)
        {
            print($"Error Disconnecting Wallet: {e.Message}");
        }
    }

    void OnDisconnected()
    {
        address = null;
        connectButton.SetActive(true);
        connectedButton.SetActive(false);
        connectDropdown.SetActive(false);
        connectedDropdown.SetActive(false);
    }

    // Switching Network

    public async void OnSwitchNetwork(Chain _chain)
    {

        try
        {
            print("chain switch to: " + _chain.ToString());
            ThirdwebManager.Instance.chain = _chain;
            print("wallet is metamask!!!" + wallet.ToString());
            PlayMakerGlobals.Instance.Variables.FindFsmInt("ChainId").Value = (int)_chain;
            if (wallet == Wallet.MetamaskMobile)
            {
                print("wallet is metamask");
                SwitchChain(_chain);
            }
            else
            {
                await ThirdwebManager.Instance.SDK.wallet.SwitchNetwork((int)_chain);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("IsNetworkSwitched").Value = true;
                OnConnected();
                if (OnSwitchNetworkCallback != null)
                    OnSwitchNetworkCallback.Invoke();
                print($"Switched Network Successfully: {_chain}");
            }

        }
        catch (Exception e)
        {
            print($"Error Switching Network: {e.Message}");
        }
    }

    public async void SwitchChain(Chain _chain)
    {
        string chainHex = _chain == Chain.MantleTestnet ? "0x1389" : "0x12";

        var data = new SwitchChainParams
        {
            ChainId = chainHex
        };

        var request = new MetaMaskEthereumRequest
        {
            Method = "wallet_switchEthereumChain",
            Parameters = new SwitchChainParams[] { data }
        };
        // onTransactionSent?.Invoke(this, EventArgs.Empty);
        try
        {
            await MetaMaskUnity.Instance.Wallet.Request(request);
            PlayMakerGlobals.Instance.Variables.FindFsmBool("IsNetworkSwitched").Value = true;
            OnConnected();
            if (OnSwitchNetworkCallback != null)
                OnSwitchNetworkCallback.Invoke();
            print($"Switched Network Successfully: {_chain}");
        }
        catch (Exception e)
        {
            print("Error switching chain: " + e.Message);
            AddEthereumChain(_chain);
        }
    }

    public async void AddEthereumChain(Chain _chain)
    {
        AddEthereumChainParams data = null;
        if (_chain == Chain.MantleTestnet)
        {

            data = new AddEthereumChainParams
            {
                ChainId = "0x1389",
                ChainName = "Mantle Testnet",
                RpcUrls = new string[] { "https://rpc.testnet.mantle.xyz" },
                NativeCurrency = new NativeCurrencyCustom
                {
                    Name = "BIT",
                    Symbol = "BIT",
                    Decimals = 18
                },
                BlockExplorerUrls = new string[] { "https://explorer.testnet.mantle.xyz" }
            };
        }
        else
        {

            data = new AddEthereumChainParams
            {
                ChainId = "0x12",
                ChainName = "ThunderCore Testnet",
                RpcUrls = new string[] { "https://testnet-rpc.thundercore.com" },
                NativeCurrency = new NativeCurrencyCustom
                {
                    Name = "TST",
                    Symbol = "TST",
                    Decimals = 18
                },
                BlockExplorerUrls = new string[] { "https://explorer-testnet.thundercore.com" }
            };
        }
        var request = new MetaMaskEthereumRequest
        {
            Method = "wallet_addEthereumChain",
            Parameters = new AddEthereumChainParams[] { data }
        };
        // onTransactionSent?.Invoke(this, EventArgs.Empty);
        try
        {
            await MetaMaskUnity.Instance.Wallet.Request(request);
            OnConnected();
            if (OnSwitchNetworkCallback != null)
                OnSwitchNetworkCallback.Invoke();
            print($"Switched Network Successfully: {_chain}");

        }
        catch (Exception e)
        {
            print("Error adding chain: " + e.Message);
        }

    }


    public void OnChainIdChanged(object sender, EventArgs e)
    {
        print("Chain Id Changed");
        OnConnected();
    }

    // UI

    public void OnClickDropdown()
    {
        if (String.IsNullOrEmpty(address))
            connectDropdown.SetActive(!connectDropdown.activeInHierarchy);
        else
            connectedDropdown.SetActive(!connectedDropdown.activeInHierarchy);
    }

    public void OnClickNetworkSwitch()
    {
        if (networkDropdown.activeInHierarchy)
        {
            networkDropdown.SetActive(false);
            return;
        }

        networkDropdown.SetActive(true);

        foreach (Transform child in networkDropdown.transform)
            Destroy(child.gameObject);

        foreach (Chain chain in Enum.GetValues(typeof(Chain)))
        {
            if (chain == ThirdwebManager.Instance.chain || !ThirdwebManager.Instance.supportedNetworks.Contains(chain))
                continue;

            GameObject networkButton = Instantiate(networkButtonPrefab, networkDropdown.transform);
            networkButton.GetComponent<Button>().onClick.RemoveAllListeners();
            networkButton.GetComponent<Button>().onClick.AddListener(() => OnSwitchNetwork(chain));
            networkButton.transform.Find("Text_Network").GetComponent<TMP_Text>().text = ThirdwebManager.Instance.chainIdentifiers[chain];
            networkButton.transform.Find("Icon_Network").GetComponent<Image>().sprite = networkSprites.Find(x => x.chain == chain).sprite;
        }
    }

    // Utility

    WalletProvider GetWalletProvider(Wallet _wallet)
    {
        switch (_wallet)
        {
            case Wallet.MetaMask:
                return WalletProvider.MetaMask;
            case Wallet.Injected:
                return WalletProvider.Injected;
            case Wallet.CoinbaseWallet:
                return WalletProvider.CoinbaseWallet;
            case Wallet.WalletConnect:
                return WalletProvider.WalletConnect;
            case Wallet.MagicAuth:
                return WalletProvider.MagicAuth;
            default:
                throw new UnityException($"Wallet Provider for wallet {_wallet} unimplemented!");
        }
    }
}