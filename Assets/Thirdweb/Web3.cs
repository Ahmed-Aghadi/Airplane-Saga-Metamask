using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Thirdweb;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

public class Web3 : MonoBehaviour
{
    string adminAddress = "0x0de82DCC40B8468639251b089f8b4A4400022e04";

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
    public async void setGoldBalance()
    {
        PlayMakerGlobals.Instance.Variables.FindFsmFloat("GOLD").Value = await getGoldBalance();
    }

    public async Task<float> getGoldBalance()
    {
        var bal = await GetGoldTokenDrop().ERC20.Balance();
        float balance = float.Parse(bal.displayValue, System.Globalization.CultureInfo.InvariantCulture);
        return balance;

    }

    public async void giveGoldBalance(string amount)
    {
        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
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

        setGoldBalance();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    private Contract GetGoldTokenDrop()
    {
        return ThirdwebManager.Instance.SDK.GetContract("0x05c135b3B0Da5B1398874F252b57a8d19cf60008");
    }
    public async void setDiamondBalance()
    {
        PlayMakerGlobals.Instance.Variables.FindFsmFloat("DIAMOND").Value = await getDiamondBalance();
    }

    public async Task<float> getDiamondBalance()
    {
        var bal = await GetDiamondTokenDrop().ERC20.Balance();
        float balance = float.Parse(bal.displayValue, System.Globalization.CultureInfo.InvariantCulture);
        return balance;

    }

    public async void buyDiamondToken(string amount)
    {
        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
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

        setDiamondBalance();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    private Contract GetDiamondTokenDrop()
    {
        return ThirdwebManager.Instance.SDK.GetContract("0x631c8C1342553EdE72a74c29B3475bf878AE3Cd7");
    }
    public async void decreaseGoldToken(string amount)
    {
        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
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

        setGoldBalance();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void decreaseDiamondToken(string amount)
    {
        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;
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

        setDiamondBalance();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void setOwnSkin1()
    {
        string tokenId = "0";
        // First, check to see if the you own the NFT
        var owned = await GetEdition().ERC1155.GetOwned();

        // if owned contains a token with the same ID as the listing, then you own it
        bool ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVESKIN1").Value = ownsNft;
    }
    public async void setOwnSkin2()
    {
        string tokenId = "1";
        // First, check to see if the you own the NFT
        var owned = await GetEdition().ERC1155.GetOwned();

        // if owned contains a token with the same ID as the listing, then you own it
        bool ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVESKIN2").Value = ownsNft;
    }

    public async void setOwnSkin3()
    {
        string tokenId = "2";
        // First, check to see if the you own the NFT
        var owned = await GetEdition().ERC1155.GetOwned();

        // if owned contains a token with the same ID as the listing, then you own it
        bool ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVESKIN3").Value = ownsNft;
    }

    public async void setOwnSkin4()
    {
        string tokenId = "3";
        // First, check to see if the you own the NFT
        var owned = await GetEdition().ERC1155.GetOwned();

        // if owned contains a token with the same ID as the listing, then you own it
        bool ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVESKIN4").Value = ownsNft;
    }

    public async void setOwnEngine1()
    {
        string tokenId = "4";
        // First, check to see if the you own the NFT
        var owned = await GetEdition().ERC1155.GetOwned();

        // if owned contains a token with the same ID as the listing, then you own it
        bool ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVEENGINE1").Value = ownsNft;
    }

    public async void setOwnEngine2()
    {
        string tokenId = "5";
        // First, check to see if the you own the NFT
        var owned = await GetEdition().ERC1155.GetOwned();

        // if owned contains a token with the same ID as the listing, then you own it
        bool ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVEENGINE2").Value = ownsNft;
    }

    public async void setOwnEngine3()
    {
        string tokenId = "6";
        // First, check to see if the you own the NFT
        var owned = await GetEdition().ERC1155.GetOwned();

        // if owned contains a token with the same ID as the listing, then you own it
        bool ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVEENGINE3").Value = ownsNft;
    }

    public async void setOwnMachineGun1()
    {
        string tokenId = "7";
        // First, check to see if the you own the NFT
        var owned = await GetEdition().ERC1155.GetOwned();

        // if owned contains a token with the same ID as the listing, then you own it
        bool ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVEMACHINEGUN1").Value = ownsNft;
    }

    public async void setOwnMachineGun2()
    {
        string tokenId = "8";
        // First, check to see if the you own the NFT
        var owned = await GetEdition().ERC1155.GetOwned();

        // if owned contains a token with the same ID as the listing, then you own it
        bool ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVEMACHINEGUN2").Value = ownsNft;
    }

    public async void setOwnMachineGun3()
    {
        string tokenId = "9";
        // First, check to see if the you own the NFT
        var owned = await GetEdition().ERC1155.GetOwned();

        // if owned contains a token with the same ID as the listing, then you own it
        bool ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVEMACHINEGUN3").Value = ownsNft;
    }

    public async void setOwnMissiles()
    {
        string tokenId = "10";
        // First, check to see if the you own the NFT
        var owned = await GetEdition().ERC1155.GetOwned();

        // if owned contains a token with the same ID as the listing, then you own it
        bool ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVEMISSILES").Value = ownsNft;
    }

    public async void setOwnBomb()
    {
        string tokenId = "11";
        // First, check to see if the you own the NFT
        var owned = await GetEdition().ERC1155.GetOwned();

        // if owned contains a token with the same ID as the listing, then you own it
        bool ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVEBOMB").Value = ownsNft;
    }

    public async void setOwnNitro1()
    {
        string tokenId = "12";
        // First, check to see if the you own the NFT
        var owned = await GetEdition().ERC1155.GetOwned();

        // if owned contains a token with the same ID as the listing, then you own it
        bool ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVENITRO1").Value = ownsNft;
    }

    public async void setOwnNitro2()
    {
        string tokenId = "13";
        // First, check to see if the you own the NFT
        var owned = await GetEdition().ERC1155.GetOwned();

        // if owned contains a token with the same ID as the listing, then you own it
        bool ownsNft = owned.Exists(nft => nft.metadata.id == tokenId);

        PlayMakerGlobals.Instance.Variables.FindFsmBool("PLAYERHAVENITRO2").Value = ownsNft;
    }

    private Contract GetEdition()
    {
        return ThirdwebManager.Instance.SDK.GetContract("0x59aA450B296c50553C5A02123ADb7a9B4BB92f5b");
    }

    public async void buySkin1Gold()
    {
        string listingId = "1";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setGoldBalance();
        setOwnSkin1();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buySkin2Gold()
    {
        string listingId = "2";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setGoldBalance();
        setOwnSkin2();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buySkin3Gold()
    {
        string listingId = "3";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setGoldBalance();
        setOwnSkin3();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buySkin4Gold()
    {
        string listingId = "4";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setGoldBalance();
        setOwnSkin4();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyEngine1Gold()
    {
        string listingId = "5";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setGoldBalance();
        setOwnEngine1();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyEngine2Gold()
    {
        string listingId = "6";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setGoldBalance();
        setOwnEngine2();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyEngine3Gold()
    {
        string listingId = "7";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setGoldBalance();
        setOwnEngine3();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyMachineGun1Gold()
    {
        string listingId = "8";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setGoldBalance();
        setOwnMachineGun1();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyMachineGun2Gold()
    {
        string listingId = "9";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setGoldBalance();
        setOwnMachineGun2();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyMachineGun3Gold()
    {
        string listingId = "10";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setGoldBalance();
        setOwnMachineGun3();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyMissilesGold()
    {
        string listingId = "11";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setGoldBalance();
        setOwnMissiles();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyBombGold()
    {
        string listingId = "12";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setGoldBalance();
        setOwnBomb();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyNitro1Gold()
    {
        string listingId = "13";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setGoldBalance();
        setOwnNitro1();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyNitro2Gold()
    {
        string listingId = "14";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setGoldBalance();
        setOwnNitro2();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buySkin1Diamond()
    {
        string listingId = "15";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setDiamondBalance();
        setOwnSkin1();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buySkin2Diamond()
    {
        string listingId = "16";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setDiamondBalance();
        setOwnSkin2();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buySkin3Diamond()
    {
        string listingId = "17";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setDiamondBalance();
        setOwnSkin3();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buySkin4Diamond()
    {
        string listingId = "18";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setDiamondBalance();
        setOwnSkin4();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyEngine1Diamond()
    {
        string listingId = "19";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setDiamondBalance();
        setOwnEngine1();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyEngine2Diamond()
    {
        string listingId = "20";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setDiamondBalance();
        setOwnEngine2();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyEngine3Diamond()
    {
        string listingId = "21";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setDiamondBalance();
        setOwnEngine3();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyMachineGun1Diamond()
    {
        string listingId = "22";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setDiamondBalance();
        setOwnMachineGun1();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyMachineGun2Diamond()
    {
        string listingId = "23";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setDiamondBalance();
        setOwnMachineGun2();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyMachineGun3Diamond()
    {
        string listingId = "24";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setDiamondBalance();
        setOwnMachineGun3();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyMissilesDiamond()
    {
        string listingId = "25";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setDiamondBalance();
        setOwnMissiles();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyBombDiamond()
    {
        string listingId = "26";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setDiamondBalance();
        setOwnBomb();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyNitro1Diamond()
    {
        string listingId = "27";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setDiamondBalance();
        setOwnNitro1();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    public async void buyNitro2Diamond()
    {
        string listingId = "28";

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = true;

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

        setDiamondBalance();
        setOwnNitro2();

        PlayMakerGlobals.Instance.Variables.FindFsmBool("LOADING").Value = false;
    }

    private Marketplace GetMarketplace()
    {
        return ThirdwebManager.Instance.SDK
            .GetContract("0xD687305714E0B84661860e94352dFDE259427591")
            .marketplace;
    }
}
