# Automated Folder Master

![Delete Button](https://i.imgur.com/XcrFxC3.png)

## What am I?

An application that automates deletion of certain files and folders, upon their passing of a certain age.

## Tech Used

Sweet and simple:
- C#
- WPF

Data binding was used in addition to delegate commands.

## Parts

### Library
* Reading and Saving Data
* Serialization to and from XML
* Holds Entities used by other parts

### Console
* Can be run manually, or started automatically
* Clears the folders' contents, or the folders themselves
* Follows the settings set up by the View
* Reads said settings from the settings file

### View
* Is a WPF front-end
* Used to set up various settings for the Console
* Initiates auto-startup
* Loosely based on MVVM pattern

## Features

`Life-time: Refers to the time the file or folder can exist up to.`

The User can set up the Console application manually, using it on one specific folder, once. This requires answers to specific questions and can be rather tedious. Upon success the application releases a heartfelt, lovely beep.

It is better to use the View, which sets up the settings file for the Console, and saves it using methods of the Library. The setup includes:

1. Selection and Addition of folders
2. Setting specific on/off toggles:
3. Editing specific folder values after addition
4. Setting global life-time
5. Setting life-time of currently added folder without another entry
6. Resetting Default Settings
7. Clearing all paths
8. Usage guide - for getting used to
9. Reloading / Saving settings

In addition, error handling (EH) has been implemented in both Console and View applications. The Console, while notifying the User, also logs selected errors. The View, for a better UX and more responsive UI, notifies the user of successful or failed operations via popups.

This also slows and disincentivizes spamming of certain features.

Last but not least, drives and special folders cannot be added / monitored, due to safety reasons.

## Usage

- Clone the repo
- Read the 'Automated_Folder_Master_Settings.sln' in Visual Studio
- Run the application

Alternatively:
- Clone the 'Release' branch / download the files
- Copy the files to the desired folder
- The folders 'View' and 'Console' should be in the same parent folder
- Create a shortcut to the View's executable, and start that

## Code Example

Creating or opening a registry key:
```
  private static RegistryKey OpenKey()
  {
      var keyLocation = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
      var setWriteable = true;

      return Registry.CurrentUser.CreateSubKey(keyLocation, setWriteable);
  }
```

## Miscellaneous
[Img Source](https://www.consumerreports.org/privacy/how-to-delete-online-accounts-you-no-longer-need/)
