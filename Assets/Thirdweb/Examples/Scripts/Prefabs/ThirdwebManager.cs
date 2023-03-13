using UnityEngine;
using Thirdweb;
using System.Collections.Generic;

[System.Serializable]
public enum Chain
{
    Ethereum = 1,
    Goerli = 5,
    Polygon = 137,
    Mumbai = 80001,
    Fantom = 250,
    FantomTestnet = 4002,
    Avalanche = 43114,
    AvalancheTestnet = 43113,
    Optimism = 10,
    OptimismGoerli = 420,
    Arbitrum = 42161,
    ArbitrumGoerli = 421613,
    Binance = 56,
    BinanceTestnet = 97,
    ThunderCoreTestnet = 18,
    MantleTestnet = 5001
}

public class ThirdwebManager : MonoBehaviour
{
    [Header("SETTINGS")]
    public Chain chain = Chain.MantleTestnet;
    public List<Chain> supportedNetworks;

    public Dictionary<Chain, string> chainIdentifiers = new Dictionary<Chain, string>
    {
        {Chain.Ethereum, "ethereum"},
        {Chain.Goerli, "goerli"},
        {Chain.Polygon, "polygon"},
        {Chain.Mumbai, "mumbai"},
        {Chain.Fantom, "fantom"},
        {Chain.FantomTestnet, "fantom-testnet"},
        {Chain.Avalanche, "avalanche"},
        {Chain.AvalancheTestnet, "avalanche-testnet"},
        {Chain.Optimism, "optimism"},
        {Chain.OptimismGoerli, "optimism-goerli"},
        {Chain.Arbitrum, "arbitrum"},
        {Chain.ArbitrumGoerli, "arbitrum-goerli"},
        {Chain.Binance, "binance"},
        {Chain.BinanceTestnet, "binance-testnet"},
        {Chain.ThunderCoreTestnet, "thundercore-testnet"},
        {Chain.MantleTestnet, "mantle-testnet"}
    };

    public ThirdwebSDK SDK;

    public static ThirdwebManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);

#if !UNITY_EDITOR
        if(chain == Chain.ThunderCoreTestnet) {
            SDK = new ThirdwebSDK("https://thundercore-testnet.rpc.thirdweb.com/ed043a51ae23b0db3873f5a38b77ab28175fa496f15d3c53cf70401be89b622a");
        } else if(chain == Chain.MantleTestnet){
            SDK = new ThirdwebSDK("https://mantle-testnet.rpc.thirdweb.com/ed043a51ae23b0db3873f5a38b77ab28175fa496f15d3c53cf70401be89b622a");
        }else {
                    SDK = new ThirdwebSDK(chainIdentifiers[chain]);
        }
#endif
    }

}
