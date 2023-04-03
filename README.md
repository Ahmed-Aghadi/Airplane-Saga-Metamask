# Airplane-Saga

It's a game where user have to protect different things like Oil Platform, Air Ship or Ruins in the open ocean or sky from the moving explosive mines by killing those mines using upgradable airplane.

User will be rewarded in Gold Tokens after completing the task. User can also buy Diamond tokens if they want too. Tokens can be used to buy Airplane skins, engines, machine guns, missiles and nitros to upgrade the airplane.

## How we built it

We have used Unity, Thirdweb, Mantle and ThunderCore to build the game and deploy the smart contracts.

Game supports 3 wallets:

-   Metamask
-   Coinbase Wallet
-   WalletConnect

Game supports 2 Chains:

-   Mantle
-   ThunderCore

Game have 3 levels:

-   Protect Oil Platform
-   Protect Air Ship
-   Protect Ruin

The Tokens and NFTs are deployed on Mantle and ThunderCore.

## Metamask SDK

### Scripts

### Connect Wallet Script

This script contains the functionality for connecting the wallet, switching chain ( or add the chain if it's not available ) and etc.

[Connect Wallet Script](https://github.com/Ahmed-Aghadi/Airplane-Saga-Metamask/blob/main/Assets/Thirdweb/Examples/Scripts/Prefabs/Prefab_ConnectWallet.cs)

##### Example

[Switch Ethereum Chain](https://github.com/Ahmed-Aghadi/Airplane-Saga-Metamask/blob/main/Assets/Thirdweb/Examples/Scripts/Prefabs/Prefab_ConnectWallet.cs#L433)

[Add Ethereum Chain](https://github.com/Ahmed-Aghadi/Airplane-Saga-Metamask/blob/main/Assets/Thirdweb/Examples/Scripts/Prefabs/Prefab_ConnectWallet.cs#L464)

[Fetch native token balance](https://github.com/Ahmed-Aghadi/Airplane-Saga-Metamask/blob/main/Assets/Thirdweb/Examples/Scripts/Prefabs/Prefab_ConnectWallet.cs#L228)

### Web3 Script

This script contains the functionality for interacting with erc20, erc1155 and marketplace contracts. Like fetching token balance, to allowing marketplace to spend tokens for buying nft using erc20 tokens, claiming erc20 tokens, etc.

[Web3 Script](https://github.com/Ahmed-Aghadi/Airplane-Saga-Metamask/blob/main/Assets/Thirdweb/Web3.cs)

##### Example

[Fetch token balance](https://github.com/Ahmed-Aghadi/Airplane-Saga-Metamask/blob/main/Assets/Thirdweb/Web3.cs#L235)

[Buy token using claim function](https://github.com/Ahmed-Aghadi/Airplane-Saga-Metamask/blob/main/Assets/Thirdweb/Web3.cs#L261)

[function to check balanceOf NFT](https://github.com/Ahmed-Aghadi/Airplane-Saga-Metamask/blob/main/Assets/Thirdweb/Web3.cs#L513)

[Fetch allowance of erc20 token for marketplace](https://github.com/Ahmed-Aghadi/Airplane-Saga-Metamask/blob/main/Assets/Thirdweb/Web3.cs#L851)

[Approve erc20 tokens to marketplace](https://github.com/Ahmed-Aghadi/Airplane-Saga-Metamask/blob/main/Assets/Thirdweb/Web3.cs#L876)

[Buy NFT using erc20 tokens from marketplace](https://github.com/Ahmed-Aghadi/Airplane-Saga-Metamask/blob/main/Assets/Thirdweb/Web3.cs#L904)

#### Smart Contracts:

##### Mantle Testnet

ERC1155 Contract containg NFT Collection:
[NFT COLLECTION](https://thirdweb.com/mantle-testnet/0x9574E60E8aBeb8062CD3DCC3ed7714E067768a72/nfts)

Marketplace when all the NFTs are listed:
[MARKETPLACE](https://thirdweb.com/mantle-testnet/0xCC571a70C092d1224e4A9f8013B66009301864E5/listings)

ERC20 token which will be given to user as a reward:
[GOLD TOKEN](https://thirdweb.com/mantle-testnet/0x11DA0f57086a19977E46B548b64166411d839a30/tokens)

ERC20 token which user can buy:
[DIAMOND TOKEN](https://thirdweb.com/mantle-testnet/0x489d47E592639Ba11107E84dd6CCA08F0892E27d/tokens)

##### ThunderCore Testnet

ERC1155 Contract containg NFT Collection:
[NFT COLLECTION](https://thirdweb.com/thundercore-testnet/0x04B8D96d7266adcb8fF45a0Eb8AFB91D79e58481/nfts)

Marketplace when all the NFTs are listed:
[MARKETPLACE](https://thirdweb.com/thundercore-testnet/0x9574E60E8aBeb8062CD3DCC3ed7714E067768a72/listings)

ERC20 token which will be given to user as a reward:
[GOLD TOKEN](https://thirdweb.com/thundercore-testnet/0x4B03368f666fa7579BfeB49eF1c5E405389b174e/tokens)

ERC20 token which user can buy:
[DIAMOND TOKEN](https://thirdweb.com/thundercore-testnet/0x7c6822e60bD40ED9202d888344628f891bA2f0f8/tokens)
