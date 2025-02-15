# ScnNavigator
This tool renders text from the game "Time Travelers" as they would show in-game.

## Configuration

The tool needs to load data from the patched and original state of the game to properly render texts and graphics.<br>
Along with that, it also needs to know for which platform it renders, as there are differences between the 3DS and, for example, PSP layouts.

All configurations are to be done in `config.json`, explained below:

|Category|Key|Description|
|--|--|--|
|`Logic.Business.TimeTravelersManagement`|`Platform`|Can either be `Ctr` or `Psp` to denote the platform to render for. Only `Ctr` is fully supported, with partial support for `Psp`.|
|`Logic.Business.TimeTravelersManagement`|`OriginalPath`|The absolute folder path to a fully extracted and complete tt1.cpk of the unmodified files from the game.|
|`Logic.Business.TimeTravelersManagement`|`PatchPath`|The absolute folder path to a fully extracted and complete tt1.cpk of the modified files from the game. May only contain a subset of the files from `OriginalPath`.|
|`Logic.Business.TimeTravelersManagement`|`PatchMapPath`|The file path, relative to ScnNavigator, that contains a mapping of font characters. The same mapping used in [FontPatcher](https://github.com/Time-Travelers-Translation/FontPatcher).|
|`Logic.Business.TranslationManagement`|`TranslationSheetId`|The ID of the Google Sheet to request translations from and persist translations to. Read [here](https://stackoverflow.com/questions/36061433/how-do-i-locate-a-google-spreadsheet-id) to learn how to retrieve the ID from your Google Sheet.|
|`Logic.Domain.GoogleSheetsManagement.OAuth2`|`ClientId`|The Client ID of an OAuth2 authentication pair to request data from and send data to Google Sheets via the API. Read [here](https://developers.google.com/identity/protocols/oauth2) to learn about the Google API and creating OAuth2 credentials.|
|`Logic.Domain.GoogleSheetsManagement.OAuth2`|`ClientSecret`|The Client Secret of an OAuth2 authentication pair to request data from and send data to Google Sheets via the API. Read [here](https://developers.google.com/identity/protocols/oauth2) to learn about the Google API and creating OAuth2 credentials.|
|`UI.ScnNavigator.Resources`|`DefaultLocale`|The default langauge for app-specific texts and descriptions as an Alpha-2 (ISO 639-1) code. You can add new languages by adding a new json for the language, based on `en.json`.|

## Story

Drag'n'Drop the file `tt1.scn` from the original game files on the tool to load the main story text and a graph view for scene flow.<br>
This will load all 7 chapters, along with Bad Ends and Decisions.

Navigate through the story by selecting the chapter tab and then left-clicking a block in the graph view to load this scene's text.<br>
It can take some seconds to load the scene text and its translations from the Google Sheet, so be patient.

You can find scenes by using `Ctrl+F` and typing in the scene identifier of the currently selected chapter.

## TIPS

Drag'n'Drop the file `Tip_List_ja.cfg.bin` from the original game files on this tool to load all TIPS and their titles.<br>
TIPS describe terminology, facts, and background information on the in-game world.

You can find TIPS by using `Ctrl+F` and typing in the TIP title.

## Help

Drag'n'Drop the file `Help_List_ja.cfg.bin` from the original game files on this tool to load all Helps and their titles.<br>
Helps describe gameplay mechanics and the basic premise of the game.

## Tutorial

Drag'n'Drop the file `Tuto_List_ja.cfg.bin` from the original game files on this tool to load all Tutorials and their titles.<br>
Tutorials guide the player during gameplay and menus.

## Staffroll

Drag'n'Drop the file `staffroll_ja.cfg.bin` from the original game files on this tool to load all Staffroll items.<br>
Those are all the lines seen in the credits at the end of the game.

Attention: The staffroll is severely limited in adding more lines and could lead to crashing the game, if not tested properly.

## Outlines

Drag'n'Drop any file `OUTLINE_[x]_ja.cfg.bin` from the original game files on this tool to load all outline texts for a given route.<br>
Those give a 2-page summary for every full hour in the game per route.

## Images

![story_text_1](https://github.com/user-attachments/assets/b799fa60-b228-488c-97c8-e7567695572d)
![story_text_2](https://github.com/user-attachments/assets/8a0d048f-d8e9-4446-b691-f0b501d98fcb)
![story_text_3](https://github.com/user-attachments/assets/67792476-106f-4e23-a1e3-abe279cc8fb1)
![tip_texts_1](https://github.com/user-attachments/assets/7b39660a-32c3-4fae-b69a-7ced5f081fd9)
![help_texts_1](https://github.com/user-attachments/assets/f29e3c4c-0aa5-4697-b7cb-25f4557b3ee2)
![tuto_texts_1](https://github.com/user-attachments/assets/28c04a4e-3c6a-44e1-8788-73646c15c6d8)
![staffroll_texts_1](https://github.com/user-attachments/assets/7f401dee-1b42-4008-99d1-bee351a38932)
![outline_texts_1](https://github.com/user-attachments/assets/9b19610f-3036-43b8-93ba-fda6d48595ac)
