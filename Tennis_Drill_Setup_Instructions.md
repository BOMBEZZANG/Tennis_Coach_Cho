# Tennis Lesson Drill Mini-Game Setup Instructions

## Overview
This document provides step-by-step instructions for setting up the Tennis Lesson Drill Mini-Game in your Unity scene.

## Required Components

### 1. Game Objects Hierarchy
Create the following GameObject structure in your scene:

```
TennisDrillMiniGame (Empty GameObject)
├── GameManager (TennisDrillMiniGame script)
├── Court
│   ├── TennisBall (with TennisBall script)
│   │   ├── BallSprite (Visual representation)
│   │   └── BallShadow (Shadow sprite)
│   ├── PlayerPaddle (with TennisPlayerPaddle script)
│   │   └── PaddleSprite (Visual representation)
│   ├── StudentPaddle (with TennisAIPaddle script)
│   │   └── PaddleSprite (Visual representation)
│   ├── JudgmentZones
│   │   ├── PerfectZone (BoxCollider2D, IsTrigger = true)
│   │   └── GoodZone (BoxCollider2D, IsTrigger = true)
│   └── PositionMarkers
│       ├── PlayerStartPosition (Empty GameObject)
│       ├── StudentStartPosition (Empty GameObject)
│       └── BallStartPosition (Empty GameObject)
└── UI
    └── TennisDrillCanvas (Canvas with TennisDrillUI script)
        ├── HUD Panel
        ├── Countdown Panel
        ├── Results Panel
        └── Waiting Panel
```

## Step-by-Step Setup

### Step 1: Create Main Game Manager
1. Create an empty GameObject named "TennisDrillMiniGame"
2. Add the `TennisDrillMiniGame` script to it
3. This will be your main manager for the mini-game

### Step 2: Set Up the Tennis Ball
1. Create a GameObject named "TennisBall"
2. Add `TennisBall` script
3. Add `CircleCollider2D` component (IsTrigger = true, Radius = 0.3)
4. Create child objects:
   - "BallSprite": Add SpriteRenderer with tennis ball sprite
   - "BallShadow": Add SpriteRenderer with shadow sprite (darker, semi-transparent)

### Step 3: Set Up Player Paddle
1. Create a GameObject named "PlayerPaddle"
2. Add `TennisPlayerPaddle` script
3. Add `BoxCollider2D` component (IsTrigger = true, Size = 0.8 x 1.2)
4. Create child object "PaddleSprite" with SpriteRenderer
5. Position at approximately (-6, 0, 0) - left side of court

### Step 4: Set Up AI Student Paddle
1. Create a GameObject named "StudentPaddle"
2. Add `TennisAIPaddle` script
3. Add `BoxCollider2D` component (IsTrigger = true, Size = 0.8 x 1.2)
4. Create child object "PaddleSprite" with SpriteRenderer
5. Position at approximately (6, 0, 0) - right side of court

### Step 5: Create Judgment Zones
1. Create empty GameObject "PerfectZone"
   - Add `BoxCollider2D` (IsTrigger = true, Size = 2 x 1.5)
   - Position on student's side: (5, 0, 0)
   
2. Create empty GameObject "GoodZone"
   - Add `BoxCollider2D` (IsTrigger = true, Size = 4 x 3)
   - Position on student's side: (5, 0, 0)
   - Should fully contain PerfectZone

### Step 6: Set Up Position Markers
Create empty GameObjects for positioning:
- **PlayerStartPosition**: (-5, 0, 0)
- **StudentStartPosition**: (5, 0, 0)
- **BallStartPosition**: (0, 0, 0)

### Step 7: Create UI Canvas
1. Create UI Canvas named "TennisDrillCanvas"
2. Add `TennisDrillUI` script to the Canvas
3. Set Canvas to "Screen Space - Overlay"

### Step 8: Set Up UI Panels

#### HUD Panel (Always visible during game)
Create Panel named "HUD Panel" with:
- **TimerText**: TextMeshPro - "Time: 01:00"
- **ComboText**: TextMeshPro - "Combo: x0"
- **ScoreText**: TextMeshPro - "Score: 0"
- **JudgmentText**: TextMeshPro - Center screen, large font

#### Countdown Panel
Create Panel named "Countdown Panel" with:
- **CountdownText**: TextMeshPro - Very large, center screen

#### Results Panel
Create Panel named "Results Panel" with:
- **FinalScoreText**: TextMeshPro
- **MaxComboText**: TextMeshPro
- **GradeText**: TextMeshPro
- **ContinueButton**: Button with "Continue" text

#### Waiting Panel
Create Panel named "Waiting Panel" with:
- **WaitingText**: TextMeshPro - Instructions for player

## Component Configuration

### TennisDrillMiniGame Settings
- **Lesson Duration**: 60 seconds (default)
- **Player Start Position**: Assign PlayerStartPosition transform
- **Student Start Position**: Assign StudentStartPosition transform
- **Ball Start Position**: Assign BallStartPosition transform
- **Perfect Zone**: Assign PerfectZone transform
- **Good Zone**: Assign GoodZone transform
- **Game Camera**: Assign your main camera
- **Zoomed In Size**: 8
- **Normal Size**: 12
- **Drill UI**: Assign TennisDrillUI component

### TennisBall Settings
- **Gravity**: -20
- **Bounce Energy Loss**: 0.85
- **Base Speed**: 8
- **Serve Speed**: 10
- **Hit Speed**: 12
- **Ball Sprite**: Assign ball sprite transform
- **Ball Shadow**: Assign shadow sprite transform

### TennisPlayerPaddle Settings
- **Move Speed**: 8
- **Movement Bounds**: 4 (units up/down from start)
- **Swing Duration**: 0.3 seconds
- **Swing Cooldown**: 0.5 seconds
- **Paddle Sprite**: Assign paddle sprite transform

### TennisAIPaddle Settings
- **Reaction Delay**: 0.2 seconds
- **Max Move Speed**: 6
- **Tracking Accuracy**: 0.8 (80% accurate)
- **Movement Bounds**: 3
- **Hit Reaction Time**: 0.3 seconds
- **Hit Radius**: 1.5 units

### TennisDrillUI Settings
Assign all the UI elements you created to their respective fields in the inspector.

## Testing the Setup

### Quick Test Checklist:
1. **Game Manager**: Check that TennisDrillMiniGame script is properly configured
2. **Ball Physics**: Ball should have gravity and bounce when dropped
3. **Player Controls**: W/S should move paddle, E should swing
4. **AI Behavior**: Student paddle should track ball movement
5. **Collisions**: Ball should detect hits from paddles
6. **Judgment Zones**: Zones should be positioned on student's side
7. **UI Elements**: All UI texts should display properly
8. **Integration**: LocationPrompt should trigger mini-game for tennis courts

### Test Sequence:
1. Enter play mode
2. Go to Tennis Court A location
3. Click "Start Lesson" button (should be within 10-minute window)
4. Mini-game should transition through states:
   - PREPARING: Countdown appears
   - ACTIVE: Ball serves, paddles respond
   - ENDED: Results screen shows after timer

## Troubleshooting

### Common Issues:

**Ball doesn't move:**
- Check if TennisBall script is attached
- Verify ball has CircleCollider2D with IsTrigger = true

**Paddles don't respond:**
- Ensure scripts are attached to correct GameObjects
- Check if BoxCollider2D components are present and set as triggers

**UI doesn't show:**
- Verify Canvas is set to Screen Space - Overlay
- Check that all UI elements are assigned in TennisDrillUI inspector

**Mini-game doesn't start:**
- Ensure TennisDrillMiniGame GameObject is in scene
- Check LocationPrompt integration (location name should contain "tennis court")

**Judgment doesn't work:**
- Verify PerfectZone and GoodZone have BoxCollider2D components
- Check that zones are positioned on the student's side (positive X)

## Performance Optimization

- Use object pooling for ball trails if needed
- Optimize UI updates (avoid updating every frame)
- Consider using FixedUpdate for physics calculations
- Profile the game to identify bottlenecks

## Extending the System

The mini-game system is designed to be modular and extensible:

- Add new judgment types by extending `HitJudgment` enum
- Create different difficulty levels by adjusting AI settings
- Add power-ups or special effects
- Implement different court types with varying physics
- Add multiplayer support by replacing AI with second player

This completes the setup instructions for the Tennis Lesson Drill Mini-Game system!