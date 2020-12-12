# Spaceship Android Game
Infinite scroller android game source.

## Screenshots
![Main menu](.Docs/Images/0.png)
![PowerUp Shop](.Docs/Images/1.png)
![Game](.Docs/Images/2.png)

## How does the game work?
### 0_main.unity:
#### MainMenuController
	* controls the buttons on the main menu
	* controls the audio on the main menu

#### Canvas/0_MainMenu
	* has everything that's included in the main menu
	* HEADER: game name, currently "Placeholder Text"
	* LoadGame: button to load game
	* BOTTOM: stuff on the bottom of the screen, shop button and money display
	
#### Canvas/1_PupShop -> MainMenuShop.cs
	* gets the loaded powerups and displays them on a list in the shop
	* controls the open/close function of the shop 
	
#### ConsoleViewerCanvas
	* just a debug console drawn on top of everything

#### PowerUpManager
	* singleton. accesible everywhere. always on scene.
	* loads all powerups contained in Resources/PowerUps
	* acts as an intermediary between the ui and the powerups

### 1_game.unity:
#### LevelController
	* controls the speed increment and the rate at which the speed will increment
	
#### Player
##### PlayerController.cs
	* controls the player position on the screen
	* clamps the player pos
	* animates the spaceship for a small feedback
	* plays sfx
##### PlayerUI.cs
	* fades the curtain out
	* shows the current speed in u/s
	* shows the currentmoney
	* spawns moneytext when you destroy an asteroid
##### PowerUP_Spawner.cs
	* checks if you have any powerups and if they have gameobjects implemented on them. if they do, spawns them in position

#### Player/PlayerModel/Model
	* two types of models. the normal one, with a non broken spaceship, trails, particlesystem, powerups, and the broken one, that enables when you die and disperses the objects with the collision magnitude.
	
#### Player/CameraCanvas
	* CAMERA_RENDER no longer used
	* Game has everything that the player sees when normally playing. RoseCam not longer used
	* RetryScreen has the buttons for retrying the game and going back to the main menu
	
#### Player/Destroyer
	* barrier that destroys everything in its path. useful for asteroids to dissapear when no longer needed.

### AsteroidGenerator
	* infinitely generates asteroids with the given parameters

# Small FAQ
#### How do I create a new PowerUp?
	* create a new PowerUpSO (ScriptableObject, right click -> Create New PowerUp)
	* give its parameters
	* if it's an active powerup (like the missile shooter), give it a gameobject for instantiating on the player.
	
#### Why everytime I open the game, I have to rebuy everything?!
whilst the Load function is implemented on the powerups, I haven't implemented the load ingame YET.

#### What's this EventsCoroutiner that's creating itself on the game?
a small tool for creating quick coroutines in a function. quick and easy to use, useful for some stuff like delayed starts.