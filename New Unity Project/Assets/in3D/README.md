# in3D Avatars Unity SDK
![in3D logo](https://in3d.io/wp-content/uploads/2020/05/logo.png)

## Description
SDK contains in3D Api interface. This SDK gives access to in3D avatar models, textured, fully rigged, prepared for animations.
- We use async, so Unity under 2017 is not supported. 
- We do not use third party plugins for json deserialization or web requests. So platform support depends on Unity version.

## Features 
- login in3d user
- get list of users avatars
- get urls for avatars models

## Installation
Please follow the instructions:
### Install via Unity Package Manager

1. Open **Edit/Project Settings/Package Manager**
2. Add a new Scoped Registry (or edit the existing OpenUPM entry) for dependencies:
    ```
    Name: package.openupm.com
    URL: https://package.openupm.com
    Scope(s): com.alteracia
    ```
3. Sdd a new Scoped Registry for SDK
    ```
    Name: unity.in3d.io
    URL: https://unity.in3d.io
    Scope(s): com.in3d.sdk
    ```
4. Click **Save** (or **Apply**)
5. Install packages in PackageManager / My Registries   

**Alternatively**, merge the snippet to **Packages/manifest.json**
```json
{
  "scopedRegistries": [
    {
      "name": "package.openupm.com",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.alteracia"
      ]
    },
    {
      "name": "unity.in3d.io",
      "url": "https://unity.in3d.io",
      "scopes": [
        "com.in3d.sdk"
      ]
    }
  ],
  "dependencies": {
    "com.alteracia.altweb": "last",
    "com.alteracia.altpatterns": "last",
    "com.in3d.sdk": "last"
  }
}
```
## Samples
For Samples import:
1. Open Unity Package Manager.
2. Select **in3D.AvatarsSDK** in package list.
3. Press **import** under **Samples** spoiler.

## SetUp configurations
Configurations is a **ScriptableObjects**.  
- To create configuration: right click in **Project window -> Create -> in3D -> Configuration**.  
- For communication with server you need **serverConfiguration**
- To get User data you need **userConfiguration**.
- If you want load specific avatar use **avatarConfiguration**.  

Each configuration have specific Methods to setUp it. 
- **userConfiguration** have **LogIn** method. 
- **avatarConfiguration** have **GetAvatarFromScan** method.

## Example
To load user avatars use ``serverConfiguration.UserAvatar``

```c#
// First you need to call one of initiated Methods and get all data:
string[] avatarIds = await server.UserAvatar.GetAvatarsIds(userConfiguration);
await server.UserAvatar.GetAvatarsUrls(userConfiguration, ModelFormat.Glb);

// Than you can get data individually from Dictionary:
server.UserAvatar.Users[userConfiguration.UserId].Avatars[avatarIds[0]].Urls[(int)ModelFormat.Glb];
```

## Support
Please contact me by e-mail: alteraciaviz@gmail.com

## Roadmap


## Project status
On going
