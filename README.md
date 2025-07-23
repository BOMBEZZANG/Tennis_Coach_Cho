# Tennis Coach Cho - Playable Prototype v1.0

A Unity 2D game where you play as a freelance tennis coach, managing appointments and improving your skills through rhythm-based mini-games.

## 🎯 Core Gameplay Loop

1. **Check Appointments** - Use your smartphone to view available lessons
2. **Accept Lessons** - Schedule appointments with clients
3. **Travel to Location** - Navigate to the tennis court or other locations
4. **Complete Mini-Games** - Play rhythm-based coaching sessions
5. **Earn Rewards** - Gain cash and experience to level up
6. **Upgrade Skills** - Spend skill points to improve performance
7. **Unlock Perks** - Use perk points for powerful abilities

## 🎮 Controls

- **Arrow Keys / WASD** - Move player
- **Tab** - Open/Close smartphone
- **Space** - Hit notes in rhythm mini-game
- **E** - Interact with locations
- **Escape** - Close current UI panel

## 📱 Smartphone Apps

### Lesson Board
- View available appointments
- See client details, time, location, and rewards
- Accept appointments to add them to your schedule

### Scheduler
- View your accepted appointments
- See upcoming lessons and completed sessions
- Get notifications when appointments are starting

### Skills & Perks
- **Skills Tab**: Upgrade coaching abilities
  - **Forehand**: Increases mini-game hit zone size
  - **Friendliness**: Increases experience gained from lessons
- **Perks Tab**: Unlock powerful abilities
  - **Well-Rested**: Increases daily stamina

## ⏰ Time System

- **Game Time**: 1 game day = 10 minutes real time
- **Game Hours**: 8:00 AM to Midnight (16 hours)
- Time automatically progresses and affects appointment scheduling
- New appointments generate each day

## 🎵 Rhythm Mini-Game

- Hit **Space** when notes reach the hit zone
- Timing determines your score:
  - **Perfect**: Best score and rewards
  - **Good**: Good score and decent rewards
  - **Bad**: Minimal score and rewards
  - **Miss**: No score or rewards
- Performance affects cash and experience earned
- Forehand skill increases hit zone size for easier timing

## 📊 Progression System

### Experience & Levels
- **Coaching EXP**: Gained from completing lessons
- **Coaching Level**: Increases when EXP bar fills, awards Skill Points
- **Player Level**: Increases when skills level up, awards Perk Points

### Currency
- **Cash**: Earned from lessons, amount based on performance
- **Skill Points**: Spend on skill tree upgrades
- **Perk Points**: Spend on powerful perks

## 🏗️ Architecture Overview

### Core Systems
- **GameManager**: Central coordinator for all systems
- **TimeSystem**: Manages in-game time and day/night cycle
- **ProgressionManager**: Handles leveling, experience, and upgrades
- **AppointmentManager**: Manages lesson scheduling and availability

### Player Systems
- **PlayerController**: Handles movement and location interaction
- **LocationTrigger**: Detects when player enters/exits locations

### UI Systems
- **UIManager**: Coordinates all UI panels and interactions
- **MainHUD**: Displays cash, levels, time, and other stats
- **SmartphoneUI**: Main interface for apps and management
- **Individual Apps**: LessonBoard, Scheduler, SkillsPerks

### Mini-Game System
- **RhythmMiniGame**: Complete rhythm-based coaching sessions
- **Performance tracking**: Measures timing accuracy for rewards

### Data Management
- **GameData**: ScriptableObject containing all game state
- **SaveSystem**: Handles saving/loading game progress
- **GameSettings**: Configurable game parameters

## 🔧 Setup Instructions

1. **Create Unity Project** (Unity 2021.3 LTS or later)
2. **Import Scripts** - Copy all scripts to `Assets/Scripts/`
3. **Create GameData** - Right-click in Project → Create → TennisCoachCho → GameData
4. **Create GameSettings** - Right-click in Project → Create → TennisCoachCho → GameSettings
5. **Setup Scene**:
   - Create GameManager GameObject with GameManager script
   - Attach TimeSystem, PlayerController, ProgressionManager, UIManager, AppointmentManager
   - Create UI Canvas with MainHUD and SmartphoneUI
   - Set up locations with LocationTrigger components
6. **Assign References** - Connect all component references in inspector

## 🎨 Placeholder Assets

All visual assets should be simple geometric shapes for this prototype:
- **Player**: Square or circle
- **Locations**: Rectangles with text labels
- **UI**: Basic Unity UI elements
- **Notes**: Simple colored shapes

## 🚀 Future Expansion

The modular architecture supports easy expansion:
- Additional skill trees (Backhand, Serve, Mental Game)
- More mini-game types (Reflex, Strategy, Timing)
- Multiple locations and courts
- Weather and seasonal systems
- Client relationship mechanics
- Equipment and customization systems

## 📝 Testing Checklist

- [ ] Player can move around the world
- [ ] Time system progresses correctly (10min real = 1 game day)
- [ ] Appointments generate daily and can be accepted
- [ ] Rhythm mini-game responds to input and calculates performance
- [ ] Cash and experience are awarded based on performance
- [ ] Skill points can be spent to upgrade abilities
- [ ] Forehand skill increases hit zone size
- [ ] Friendliness skill increases experience gain
- [ ] Player levels up and gains perk points
- [ ] UI displays all relevant information
- [ ] Save/load system preserves progress

## 🔍 Core Hypotheses Being Tested

1. **Core Fun**: Is the rhythm mini-game engaging enough for repeated play?
2. **Growth Feedback**: Does the progression system feel rewarding and meaningful?
3. **System Usability**: Is the appointment scheduling system intuitive to use?

---

**Note**: This is a prototype focused on core gameplay mechanics. Polish, art, and audio are intentionally minimal to focus on testing the core game loop and progression systems.