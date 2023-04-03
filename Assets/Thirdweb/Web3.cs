using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Thirdweb;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System.Numerics;
using System.Text.Json.Serialization;
using MetaMask.Unity;
using MetaMask.Models;
using Nethereum.Signer;
using Contract = Thirdweb.Contract;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using System.Globalization;
using NBitcoin;
using Org.BouncyCastle.Utilities;
using static HutongGames.EasingFunction;
using System.Text.Json;
using Nethereum.ABI;
using UnityEngine.Analytics;

[Function("balanceOf", "uint256")]
public class BalanceOfTokenFunction : FunctionMessage
{
    [Parameter("address", "who", 1)]
    public string Who { get; set; }
}

public class EthCallParams
{

    [JsonProperty("to")]
    [JsonPropertyName("to")]
    public string To { get; set; }

    [JsonProperty("data")]
    [JsonPropertyName("data")]
    public string Data { get; set; }

}

[Function("claim")]
public class ClaimFunction : FunctionMessage
{
    //address _receiver,
    //uint256 _quantity,
    //address _currency,
    //uint256 _pricePerToken,
    //AllowlistProof calldata _allowlistProof,
    //bytes memory _data
    [Parameter("address", "_receiver", 1)]
    public string Receiver { get; set; }

    [Parameter("uint256", "_quantity", 2)]
    public BigInteger Quantity { get; set; }

    [Parameter("address", "_currency", 3)]
    public string Currency { get; set; }

    [Parameter("uint256", "_pricePerToken", 4)]
    public BigInteger PricePerToken { get; set; }

    [Parameter("tuple", "_allowlistProof", 5)]
    public AllowlistProof AllowlistProof { get; set; }

    [Parameter("bytes", "_data", 6)]
    public byte[] Data { get; set; }
}

public class EthSendTransactionParams
{
    [JsonProperty("to")]
    [JsonPropertyName("to")]
    public string To { get; set; }

    [JsonProperty("from")]
    [JsonPropertyName("from")]
    public string From { get; set; }

    [JsonProperty("data")]
    [JsonPropertyName("data")]
    public string Data { get; set; }

    [JsonProperty("value")]
    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonProperty("gas")]
    [JsonPropertyName("gas")]
    public string Gas { get; set; }

}

public class AllowlistProof
{
    [Parameter("bytes32[]", "proof", 1)]
    public byte[] Proof { get; set; }

    [Parameter("uint256", "quantityLimitPerWallet", 2)]
    public BigInteger QuantityLimitPerWallet { get; set; }

    [Parameter("uint256", "pricePerToken", 3)]
    public BigInteger PricePerToken { get; set; }

    [Parameter("address", "currency", 4)]
    public string Currency { get; set; }
}

[Function("balanceOf", "uint256")]
public class BalanceOfNFTFunction : FunctionMessage
{
    [Parameter("address", "account", 1)]
    public string Account { get; set; }

    [Parameter("uint256", "id", 2)]
    public BigInteger Id { get; set; }
}

[Function("allowance", "uint256")]
public class AllowanceFunction : FunctionMessage
{
    [Parameter("address", "owner", 1)]
    public string Owner { get; set; }

    [Parameter("address", "spender", 2)]
    public string Spender { get; set; }
}

[Function("approve", "bool")]
public class ApproveFunction : FunctionMessage
{
    [Parameter("address", "spender", 1)]
    public string Spender { get; set; }

    [Parameter("uint256", "amount", 2)]
    public BigInteger Amount { get; set; }
}

[Function("buy")]
public class BuyFunction : FunctionMessage
{
    [Parameter("uint256", "_listingId", 1)]
    public BigInteger ListingId { get; set; }

    [Parameter("address", "_buyFor", 2)]
    public string BuyFor { get; set; }

    [Parameter("uint256", "_quantityToBuy", 3)]
    public BigInteger QuantityToBuy { get; set; }

    [Parameter("address", "_currency", 4)]
    public string Currency { get; set; }

    [Parameter("uint256", "_totalPrice", 5)]
    public BigInteger TotalPrice { get; set; }
}

public class Web3 : MonoBehaviour
{
    string[] listingPriceGold = {
        "",
        "5000",
        "10000",
        "15000",
        "20000",
        "500",
        "1000",
        "1500",
        "500",
        "1000",
        "1500",
        "1000",
        "1000",
        "500",
        "1000",
    };
    string[] listingPriceDiamond = {
        "",
        "2500",
        "5000",
        "7500",
        "10000",
        "250",
        "500",
        "750",
        "250",
        "500",
        "750",
        "500",
        "500",
        "250",
        "500",
    };
    string adminAddress = "0x0de82DCC40B8468639251b089f8b4A4400022e04";
    string nativeTokenAddress = "0xeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee";
    string goldTokenMantle = "0x11DA0f57086a19977E46B548b64166411d839a30";
    string goldTokenThundercore = "0x4B03368f666fa7579BfeB49eF1c5E405389b174e";
    string diamondTokenMantle = "0x489d47E592639Ba11107E84dd6CCA08F0892E27d";
    string diamondTokenThundercore = "0x7c6822e60bD40ED9202d888344628f891bA2f0f8";
    string editionMantle = "0x9574E60E8aBeb8062CD3DCC3ed7714E067768a72";
    string editionThundercore = "0x04B8D96d7266adcb8fF45a0Eb8AFB91D79e58481";
    string marketplaceMantle = "0xCC571a70C092d1224e4A9f8013B66009301864E5";
    string marketplaceThundercore = "0x9574E60E8aBeb8062CD3DCC3ed7714E067768a72";

    public void initializeVariables()
    {
        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING0").Value = true;
        setGoldBalance();
        setDiamondBalance();
        setOwnSkin1();
        setOwnSkin2();
        setOwnSkin3();
        setOwnSkin4();
        setOwnEngine1();
        setOwnEngine2();
        setOwnEngine3();
        setOwnMachineGun1();
        setOwnMachineGun2();
        setOwnMachineGun3();
        setOwnMissiles();
        setOwnBomb();
        setOwnNitro1();
        setOwnNitro2();
        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING0").Value = false;
    }

    public async Task<float> getTokenBalance(string address)
    {
        var args = new BalanceOfTokenFunction() { Who = MetaMaskUnity.Instance.Wallet.SelectedAddress };
        var dataValue = args.GetCallData();

        var data = new EthCallParams
        {
            To = address,
            Data = "0x" + dataValue.ToHex()
        };

        var request = new MetaMaskEthereumRequest
        {
            Method = "eth_call",
            Parameters = new EthCallParams[] { data }
        };

        var response = await MetaMaskUnity.Instance.Wallet.Request(request);
        string balanceHex = response.ToString();
        var balanceHexBigInt = new HexBigInteger(balanceHex);
        var balanceBigInt = balanceHexBigInt.Value;
        var balanceEth = balanceBigInt.ToString().ToEth();
        var balance = float.Parse(balanceEth, CultureInfo.InvariantCulture.NumberFormat);
        return balance;
    }

    public async Task<JsonElement> ClaimToken(string tokenAddress, string quantity, string PricePerToken)
    {
        var args = new ClaimFunction()
        {
            Receiver = MetaMaskUnity.Instance.Wallet.SelectedAddress,
            Quantity = Nethereum.Web3.Web3.Convert.ToWei(quantity),
            Currency = nativeTokenAddress,
            PricePerToken = Nethereum.Web3.Web3.Convert.ToWei(PricePerToken),
            AllowlistProof = new AllowlistProof
            {
                Proof = new byte[0],
                QuantityLimitPerWallet = 0,
                PricePerToken = Nethereum.Web3.Web3.Convert.ToWei(PricePerToken),
                Currency = nativeTokenAddress
            },
            Data = new byte[0]
        };
        var dataValue = args.GetCallData();
        print("call data: " + dataValue);

        var value = Nethereum.Web3.Web3.Convert.ToWei(decimal.Parse(PricePerToken) * decimal.Parse(quantity));
        print("value to pass: BigInt: " + value.ToString() + " Hex: 0x" + value.ToString("X"));

        var data = new EthSendTransactionParams
        {
            To = tokenAddress,
            From = MetaMaskUnity.Instance.Wallet.SelectedAddress,
            Data = "0x" + dataValue.ToHex(),
            Value = "0x" + value.ToString("X"),
            Gas = 250000.ToString("X") // Metamask wasn't able to calculate gas by it's own
        };

        var request = new MetaMaskEthereumRequest
        {
            Method = "eth_sendTransaction",
            Parameters = new EthSendTransactionParams[] { data }
        };

        var response = await MetaMaskUnity.Instance.Wallet.Request(request);
        print("response claim token: " + response);
        return response;
    }
    public async void setGoldBalance()
    {
        PlayMakerGlobals.Instance.Variables.FindFsmFloat("GOLD").Value = await getGoldBalance();
    }

    public async Task<float> getGoldBalance()
    {
        Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
        float balance = 0;
        if (wallet.Equals(Wallet.MetamaskMobile))
        {
            balance = await getTokenBalance(GetGoldTokenAddress());
        }
        else
        {
            var bal = await GetGoldTokenDrop().ERC20.Balance();
            balance = float.Parse(bal.displayValue, System.Globalization.CultureInfo.InvariantCulture);
        }
        return balance;
    }

    public async void giveGoldBalance(string amount)
    {
        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                await ClaimToken(GetGoldTokenAddress(), amount, "0");
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetGoldTokenDrop().ERC20.Claim(amount);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }

        setGoldBalance();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    private string GetGoldTokenAddress()
    {
        if (ThirdwebManager.Instance.chain == Chain.MantleTestnet)
        {
            return goldTokenMantle;
        }
        else // for thunderCore Testnet
        {
            return goldTokenThundercore;
        }
    }

    private Contract GetGoldTokenDrop()
    {
        return ThirdwebManager.Instance.SDK.GetContract(GetGoldTokenAddress());

    }
    public async void setDiamondBalance()
    {
        PlayMakerGlobals.Instance.Variables.FindFsmFloat("DIAMOND").Value = await getDiamondBalance();
    }

    public async Task<float> getDiamondBalance()
    {
        Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
        float balance = 0;
        if (wallet.Equals(Wallet.MetamaskMobile))
        {
            balance = await getTokenBalance(GetDiamondTokenAddress());
        }
        else
        {
            var bal = await GetDiamondTokenDrop().ERC20.Balance();
            balance = float.Parse(bal.displayValue, System.Globalization.CultureInfo.InvariantCulture);
        }
        return balance;
    }

    public async void buyDiamondToken(string amount)
    {
        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                await ClaimToken(GetDiamondTokenAddress(), amount, "0.1");
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetDiamondTokenDrop().ERC20.Claim(amount);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }

        setDiamondBalance();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }


    private string GetDiamondTokenAddress()
    {
        if (ThirdwebManager.Instance.chain == Chain.MantleTestnet)
        {
            return diamondTokenMantle;
        }
        else // for thunderCore Testnet
        {
            return diamondTokenThundercore;
        }
    }

    private Contract GetDiamondTokenDrop()
    {
        return ThirdwebManager.Instance.SDK.GetContract(GetDiamondTokenAddress());
    }
    public async void decreaseGoldToken(string amount)
    {
        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            var result = await GetGoldTokenDrop().ERC20.Transfer(adminAddress, amount);

            var isSuccess = result.isSuccessful();

            if (isSuccess)
            {
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }

        setGoldBalance();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void decreaseDiamondToken(string amount)
    {
        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            var result = await GetDiamondTokenDrop().ERC20.Transfer(adminAddress, amount);

            var isSuccess = result.isSuccessful();

            if (isSuccess)
            {
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }

        setDiamondBalance();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async Task<bool> OwnNft(string address, int tokenId)
    {
        var args = new BalanceOfNFTFunction() { Account = MetaMaskUnity.Instance.Wallet.SelectedAddress, Id = tokenId };
        var dataValue = args.GetCallData();

        var data = new EthCallParams
        {
            To = address,
            Data = "0x" + dataValue.ToHex()
        };

        var request = new MetaMaskEthereumRequest
        {
            Method = "eth_call",
            Parameters = new EthCallParams[] { data }
        };

        var balanceHex = (await MetaMaskUnity.Instance.Wallet.Request(request)).ToString();
        var balanceHexBigInt = new HexBigInteger(balanceHex);
        var balanceBigInt = balanceHexBigInt.Value;
        if (balanceBigInt > 0)
        {
            return true;
        }
        return false;
    }

    public async void setOwnSkin1()
    {
        string tokenId = "0";
        bool ownsNft = false;
        Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
        if (wallet.Equals(Wallet.MetamaskMobile))
        {
            ownsNft = await OwnNft(GetEditionAddress(), Int32.Parse(tokenId));
        }
        else
        {
            // First, check to see if the you own the NFT
            var owned = await GetEdition().ERC1155.GetOwned();

            // if owned contains a token with the same ID as the listing, then you own it
            ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);
        }
        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVESKIN1").Value = ownsNft;
    }
    public async void setOwnSkin2()
    {
        string tokenId = "1";
        bool ownsNft = false;
        Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
        if (wallet.Equals(Wallet.MetamaskMobile))
        {
            ownsNft = await OwnNft(GetEditionAddress(), Int32.Parse(tokenId));
        }
        else
        {
            // First, check to see if the you own the NFT
            var owned = await GetEdition().ERC1155.GetOwned();

            // if owned contains a token with the same ID as the listing, then you own it
            ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);
        }

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVESKIN2").Value = ownsNft;
    }

    public async void setOwnSkin3()
    {
        string tokenId = "2";
        bool ownsNft = false;
        Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
        if (wallet.Equals(Wallet.MetamaskMobile))
        {
            ownsNft = await OwnNft(GetEditionAddress(), Int32.Parse(tokenId));
        }
        else
        {
            // First, check to see if the you own the NFT
            var owned = await GetEdition().ERC1155.GetOwned();

            // if owned contains a token with the same ID as the listing, then you own it
            ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);
        }

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVESKIN3").Value = ownsNft;
    }

    public async void setOwnSkin4()
    {
        string tokenId = "3";
        bool ownsNft = false;
        Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
        if (wallet.Equals(Wallet.MetamaskMobile))
        {
            ownsNft = await OwnNft(GetEditionAddress(), Int32.Parse(tokenId));
        }
        else
        {
            // First, check to see if the you own the NFT
            var owned = await GetEdition().ERC1155.GetOwned();

            // if owned contains a token with the same ID as the listing, then you own it
            ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);
        }

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVESKIN4").Value = ownsNft;
    }

    public async void setOwnEngine1()
    {
        string tokenId = "4";
        bool ownsNft = false;
        Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
        if (wallet.Equals(Wallet.MetamaskMobile))
        {
            ownsNft = await OwnNft(GetEditionAddress(), Int32.Parse(tokenId));
        }
        else
        {
            // First, check to see if the you own the NFT
            var owned = await GetEdition().ERC1155.GetOwned();

            // if owned contains a token with the same ID as the listing, then you own it
            ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);
        }

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVEENGINE1").Value = ownsNft;
    }

    public async void setOwnEngine2()
    {
        string tokenId = "5";
        bool ownsNft = false;
        Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
        if (wallet.Equals(Wallet.MetamaskMobile))
        {
            ownsNft = await OwnNft(GetEditionAddress(), Int32.Parse(tokenId));
        }
        else
        {
            // First, check to see if the you own the NFT
            var owned = await GetEdition().ERC1155.GetOwned();

            // if owned contains a token with the same ID as the listing, then you own it
            ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);
        }

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVEENGINE2").Value = ownsNft;
    }

    public async void setOwnEngine3()
    {
        string tokenId = "6";
        bool ownsNft = false;
        Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
        if (wallet.Equals(Wallet.MetamaskMobile))
        {
            ownsNft = await OwnNft(GetEditionAddress(), Int32.Parse(tokenId));
        }
        else
        {
            // First, check to see if the you own the NFT
            var owned = await GetEdition().ERC1155.GetOwned();

            // if owned contains a token with the same ID as the listing, then you own it
            ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);
        }

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVEENGINE3").Value = ownsNft;
    }

    public async void setOwnMachineGun1()
    {
        string tokenId = "7";
        bool ownsNft = false;
        Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
        if (wallet.Equals(Wallet.MetamaskMobile))
        {
            ownsNft = await OwnNft(GetEditionAddress(), Int32.Parse(tokenId));
        }
        else
        {
            // First, check to see if the you own the NFT
            var owned = await GetEdition().ERC1155.GetOwned();

            // if owned contains a token with the same ID as the listing, then you own it
            ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);
        }

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVEMACHINEGUN1").Value = ownsNft;
    }

    public async void setOwnMachineGun2()
    {
        string tokenId = "8";
        bool ownsNft = false;
        Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
        if (wallet.Equals(Wallet.MetamaskMobile))
        {
            ownsNft = await OwnNft(GetEditionAddress(), Int32.Parse(tokenId));
        }
        else
        {
            // First, check to see if the you own the NFT
            var owned = await GetEdition().ERC1155.GetOwned();

            // if owned contains a token with the same ID as the listing, then you own it
            ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);
        }

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVEMACHINEGUN2").Value = ownsNft;
    }

    public async void setOwnMachineGun3()
    {
        string tokenId = "9";
        bool ownsNft = false;
        Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
        if (wallet.Equals(Wallet.MetamaskMobile))
        {
            ownsNft = await OwnNft(GetEditionAddress(), Int32.Parse(tokenId));
        }
        else
        {
            // First, check to see if the you own the NFT
            var owned = await GetEdition().ERC1155.GetOwned();

            // if owned contains a token with the same ID as the listing, then you own it
            ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);
        }

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVEMACHINEGUN3").Value = ownsNft;
    }

    public async void setOwnMissiles()
    {
        string tokenId = "10";
        bool ownsNft = false;
        Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
        if (wallet.Equals(Wallet.MetamaskMobile))
        {
            ownsNft = await OwnNft(GetEditionAddress(), Int32.Parse(tokenId));
        }
        else
        {
            // First, check to see if the you own the NFT
            var owned = await GetEdition().ERC1155.GetOwned();

            // if owned contains a token with the same ID as the listing, then you own it
            ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);
        }

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVEMISSILES").Value = ownsNft;
    }

    public async void setOwnBomb()
    {
        string tokenId = "11";
        bool ownsNft = false;
        Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
        if (wallet.Equals(Wallet.MetamaskMobile))
        {
            ownsNft = await OwnNft(GetEditionAddress(), Int32.Parse(tokenId));
        }
        else
        {
            // First, check to see if the you own the NFT
            var owned = await GetEdition().ERC1155.GetOwned();

            // if owned contains a token with the same ID as the listing, then you own it
            ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);
        }

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVEBOMB").Value = ownsNft;
    }

    public async void setOwnNitro1()
    {
        string tokenId = "12";
        bool ownsNft = false;
        Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
        if (wallet.Equals(Wallet.MetamaskMobile))
        {
            ownsNft = await OwnNft(GetEditionAddress(), Int32.Parse(tokenId));
        }
        else
        {
            // First, check to see if the you own the NFT
            var owned = await GetEdition().ERC1155.GetOwned();

            // if owned contains a token with the same ID as the listing, then you own it
            ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);
        }

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVENITRO1").Value = ownsNft;
    }

    public async void setOwnNitro2()
    {
        string tokenId = "13";
        bool ownsNft = false;
        Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
        if (wallet.Equals(Wallet.MetamaskMobile))
        {
            ownsNft = await OwnNft(GetEditionAddress(), Int32.Parse(tokenId));
        }
        else
        {
            // First, check to see if the you own the NFT
            var owned = await GetEdition().ERC1155.GetOwned();

            // if owned contains a token with the same ID as the listing, then you own it
            ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);
        }

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVENITRO2").Value = ownsNft;
    }


    private string GetEditionAddress()
    {
        if (ThirdwebManager.Instance.chain == Chain.MantleTestnet)
        {
            return editionMantle;
        }
        else // for thunderCore Testnet
        {
            return editionThundercore;
        }
    }

    private Contract GetEdition()
    {
        return ThirdwebManager.Instance.SDK.GetContract(GetEditionAddress());

    }

    public async Task<BigInteger> Allowance(string tokenAddress, string spender)
    {
        var args = new AllowanceFunction() { Owner = MetaMaskUnity.Instance.Wallet.SelectedAddress, Spender = spender };
        var dataValue = args.GetCallData();

        var data = new EthCallParams
        {
            To = tokenAddress,
            Data = "0x" + dataValue.ToHex()
        };

        var request = new MetaMaskEthereumRequest
        {
            Method = "eth_call",
            Parameters = new EthCallParams[] { data }
        };

        var balanceHex = (await MetaMaskUnity.Instance.Wallet.Request(request)).ToString();
        print("response allowance token: " + balanceHex);
        var balanceHexBigInt = new HexBigInteger(balanceHex);
        var balanceBigInt = balanceHexBigInt.Value;
        return balanceBigInt;
    }


    public async Task<JsonElement> Approve(string tokenAddress, string spender, BigInteger amount)
    {
        var args = new ApproveFunction()
        {
            Spender = spender,
            Amount = amount
        };
        var dataValue = args.GetCallData();

        var data = new EthSendTransactionParams
        {
            To = tokenAddress,
            From = MetaMaskUnity.Instance.Wallet.SelectedAddress,
            Data = "0x" + dataValue.ToHex(),
            Value = "0x0",
        };

        var request = new MetaMaskEthereumRequest
        {
            Method = "eth_sendTransaction",
            Parameters = new EthSendTransactionParams[] { data }
        };

        var response = await MetaMaskUnity.Instance.Wallet.Request(request);
        print("response aprove token: " + response);
        return response;
    }

    public async Task<JsonElement> BuySkin(string listingId, bool isGold)
    {
        string tokenAddress = isGold ? GetGoldTokenAddress() : GetDiamondTokenAddress();
        string marketplaceAddress = GetMarketplaceAddress();
        BigInteger allowance = await Allowance(tokenAddress, marketplaceAddress);
        BigInteger priceOfNft = Nethereum.Web3.Web3.Convert.ToWei(isGold ? listingPriceGold[Int32.Parse(listingId)] : listingPriceDiamond[Int32.Parse(listingId)]);
        if (allowance < priceOfNft)
        {
            await Approve(tokenAddress, marketplaceAddress, priceOfNft);
        }
        // buy skin
        var args = new BuyFunction()
        {
            ListingId = Int32.Parse(listingId),
            BuyFor = MetaMaskUnity.Instance.Wallet.SelectedAddress,
            QuantityToBuy = 1,
            Currency = tokenAddress,
            TotalPrice = priceOfNft
        };
        var dataValue = args.GetCallData();

        var data = new EthSendTransactionParams
        {
            To = marketplaceAddress,
            From = MetaMaskUnity.Instance.Wallet.SelectedAddress,
            Data = "0x" + dataValue.ToHex(),
            Gas = 350000.ToString("X") // Metamask wasn't able to calculate gas by it's own
        };

        var request = new MetaMaskEthereumRequest
        {
            Method = "eth_sendTransaction",
            Parameters = new EthSendTransactionParams[] { data }
        };

        var response = await MetaMaskUnity.Instance.Wallet.Request(request);
        print("response buy nft: " + response);
        return response;
    }

    public async void buySkin1Gold()
    {
        string listingId = "1";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, true);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setGoldBalance();
        setOwnSkin1();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buySkin2Gold()
    {
        string listingId = "2";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, true);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setGoldBalance();
        setOwnSkin2();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buySkin3Gold()
    {
        string listingId = "3";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, true);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setGoldBalance();
        setOwnSkin3();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buySkin4Gold()
    {
        string listingId = "4";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, true);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setGoldBalance();
        setOwnSkin4();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyEngine1Gold()
    {
        string listingId = "5";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, true);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setGoldBalance();
        setOwnEngine1();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyEngine2Gold()
    {
        string listingId = "6";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, true);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setGoldBalance();
        setOwnEngine2();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyEngine3Gold()
    {
        string listingId = "7";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, true);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setGoldBalance();
        setOwnEngine3();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyMachineGun1Gold()
    {
        string listingId = "8";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, true);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setGoldBalance();
        setOwnMachineGun1();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyMachineGun2Gold()
    {
        string listingId = "9";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, true);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setGoldBalance();
        setOwnMachineGun2();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyMachineGun3Gold()
    {
        string listingId = "10";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, true);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setGoldBalance();
        setOwnMachineGun3();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyMissilesGold()
    {
        string listingId = "11";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, true);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setGoldBalance();
        setOwnMissiles();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyBombGold()
    {
        string listingId = "12";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, true);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setGoldBalance();
        setOwnBomb();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyNitro1Gold()
    {
        string listingId = "13";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, true);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setGoldBalance();
        setOwnNitro1();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyNitro2Gold()
    {
        string listingId = "14";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, true);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setGoldBalance();
        setOwnNitro2();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buySkin1Diamond()
    {
        string listingId = "15";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, false);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setDiamondBalance();
        setOwnSkin1();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buySkin2Diamond()
    {
        string listingId = "16";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, false);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setDiamondBalance();
        setOwnSkin2();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buySkin3Diamond()
    {
        string listingId = "17";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, false);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setDiamondBalance();
        setOwnSkin3();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buySkin4Diamond()
    {
        string listingId = "18";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, false);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setDiamondBalance();
        setOwnSkin4();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyEngine1Diamond()
    {
        string listingId = "19";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, false);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setDiamondBalance();
        setOwnEngine1();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyEngine2Diamond()
    {
        string listingId = "20";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, false);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setDiamondBalance();
        setOwnEngine2();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyEngine3Diamond()
    {
        string listingId = "21";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, false);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setDiamondBalance();
        setOwnEngine3();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyMachineGun1Diamond()
    {
        string listingId = "22";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, false);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setDiamondBalance();
        setOwnMachineGun1();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyMachineGun2Diamond()
    {
        string listingId = "23";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, false);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setDiamondBalance();
        setOwnMachineGun2();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyMachineGun3Diamond()
    {
        string listingId = "24";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, false);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setDiamondBalance();
        setOwnMachineGun3();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyMissilesDiamond()
    {
        string listingId = "25";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, false);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setDiamondBalance();
        setOwnMissiles();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyBombDiamond()
    {
        string listingId = "26";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, false);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setDiamondBalance();
        setOwnBomb();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyNitro1Diamond()
    {
        string listingId = "27";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, false);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setDiamondBalance();
        setOwnNitro1();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyNitro2Diamond()
    {
        string listingId = "28";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
        try
        {
            Wallet wallet = (Wallet)PlayMakerGlobals.Instance.Variables.GetFsmInt("WALLET").Value;
            if (wallet.Equals(Wallet.MetamaskMobile))
            {
                var response = await BuySkin(listingId, false);
                PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
            }
            else
            {
                var result = await GetMarketplace().BuyListing(listingId, 1);

                var isSuccess = result.isSuccessful();

                if (isSuccess)
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = true;
                }
                else
                {
                    PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
                }
            }
        }
        catch (Exception e)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmBool("WasTransactionSuccessful").Value = false;
            print($"Error: {e.Message}");
        }
        setDiamondBalance();
        setOwnNitro2();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }


    private string GetMarketplaceAddress()
    {
        if (ThirdwebManager.Instance.chain == Chain.MantleTestnet)
        {
            return marketplaceMantle;
        }
        else // for thunderCore Testnet
        {
            return marketplaceThundercore;
        }
    }

    private Marketplace GetMarketplace()
    {
        return ThirdwebManager.Instance.SDK
        .GetContract(GetMarketplaceAddress())
        .marketplace;

    }
}
