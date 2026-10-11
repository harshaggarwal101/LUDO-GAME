# 🎲 Classic Ludo 2D (Unity)

![Unity](https://img.shields.io/badge/Unity-2D-100000?style=for-the-badge&logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)

**🕹️ Play It Live:** [Experience the game directly in your browser on Unity Play!](https://play.unity.com/en/games/943da9e2-da8a-4b9e-a679-26554d0ee38c/build2)

A fully playable 2D adaptation of the classic Ludo board game, built in Unity. This project faithfully recreates the traditional rules of Ludo while adding dynamic digital features like automated turn management, dynamic token scaling, and integrated sound effects.

## 📖 About Classic Ludo

Ludo is a strategic board game derived from the ancient Indian game Pachisi. Players race their four tokens from start to finish according to the rolls of a single die. Like the traditional physical game, this digital version relies on a mix of luck (rolling the die) and strategy (choosing which piece to move, when to capture an opponent, and when to stay in a safe zone).

## 📸 Screenshots

_Menu Interface Options:_
<img src="watermarked_img_14065562416713059462.png" width="250">[cite: 2] <img src="Gemini_Generated_Image_440bkv440bkv440b.png" width="250">[cite: 1]

_Add additional screenshots of your active game board here:_

- `![Game Board](link-to-your-board-image.png)`
- `![Winning Screen](link-to-your-win-image.png)`

---

## ✨ Game Features

- **Flexible Multiplayer Modes:** Full support for 2-player, 3-player, and 4-player matches. The game automatically disables unused corners and dice based on the selected mode.
- **Intelligent Turn Management:** The game automatically passes the turn to the next player unless a bonus condition is met (e.g., rolling a 6, capturing an opponent, or reaching the center base).
- **Dynamic Board Scaling:** When multiple player pieces land on the exact same tile, the tokens automatically scale down and arrange themselves in an overlapping pattern to prevent visual clipping and ensure all pieces remain visible.
- **Safe Zones:** Designated star tiles where pieces are protected from being captured by opponents.
- **Responsive Audio:** Custom sound effects triggered for dice rolls, token footsteps, capturing an opponent (cutting), and successfully moving a piece into the final home zone.

---

## ⚙️ Feature Setup & Configuration (Unity Editor)

The game is designed to be easily configurable directly from the Unity Editor without needing to dig into the C# logic.

- **UI Panel Management:** The Main Menu (Player Selection) and Game Board UI are easily swappable via drag-and-drop references in the GameManager.
- **Audio Setup:** Sound effects for moving, capturing, and scoring can be swapped out in the GameManager's Inspector under the "Audio" header.
- **Path Generation:** The path each colored piece takes is defined by an array of `PathPoint` transforms inside the `PathObjectParent`. This allows the physical board layout to be adjusted or expanded without rewriting movement logic.
- **Player Counts:** The game automatically configures the active teams (Blue, Red, Green, Yellow) based on the UI button pressed (2, 3, or 4 players).

---

## 🎮 How to Play

1. **Start the Game:** Select the number of players from the main menu.
2. **Leave the Base:** You must roll a **6** to bring a piece out of your starting base and place it on the board.
3. **Movement:** Click your colored die to roll. Then, click the specific piece you wish to move. Tokens move clockwise around the outer perimeter of the board.
4. **Capturing (Cutting):** If your piece lands on the same tile as an opponent's piece, the opponent's piece is sent back to their base, and you receive an extra dice roll.
5. **Safe Zones:** Tokens cannot be captured if they are resting on a designated safe tile (usually marked with a star).
6. **Winning:** Navigate all 4 of your tokens completely around the board and up your designated colored path into the center home triangle. The first player to get all 4 pieces home wins.

---

## 🚀 Running the Project Locally

To test or modify the source code on your local machine:

1. **Clone the Repository:**
   ```bash
   ⏺ git clone [https://github.com/YourUsername/YourRepositoryName.git](https://github.com/YourUsername/YourRepositoryName.git)
   ```
2. **Open in Unity:**
   ⏺ Launch Unity Hub.
   ⏺ Click Add or Open and select the cloned repository folder.
   ⏺ Ensure you are using the correct Unity version (e.g., Unity 2022.3 LTS).
3. **Load and Play:**
   ⏺ Navigate to Assets/Scenes and open the main gameplay scene.
   ⏺ Press the Play button in the editor. All prefabs, variables, and public references are already connected.
