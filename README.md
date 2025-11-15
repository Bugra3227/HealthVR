# HealthVR – Color Memory Experiment in VR

**HealthVR** is a small VR prototype built with Unity, designed to explore how well players remember and follow colors in a virtual environment.

The experience takes place inside a virtual **hospital**. When the session starts, the system randomly selects one of **five colors**. The player must follow this chosen color through the environment and reach the final target. The goal is to observe **which colors are more memorable / easier to follow**.

---

## 🎯 Core Idea

- VR environment based on a hospital interior  
- At the beginning of each run, **one color is selected randomly** from a set of five  
- The chosen color appears on objects / indicators along the path  
- The player must **track and follow only that color** to reach the goal  
- The prototype is intended to help understand **which colors stay in the player’s mind more easily**

---

## 🕹 Gameplay Flow

1. The player starts in a hospital-themed VR scene.  
2. The system automatically picks one of five colors (e.g. red, blue, green, yellow, purple).  
3. The player looks for and follows that color on signs / markers in the environment.  
4. If the player successfully follows the correct color, they reach the final target area.  
5. The prototype can be used to observe which colors are more effective or memorable.

---

## 🛠 Tech Stack

- **Engine:** Unity  
- **Language:** C#  
- **Platform:** VR (e.g. Meta Quest / PC VR)  
- **Input:** VR controllers / head tracking  

> Note: This project was created as a private VR experience and is **not published on any store**.

---

## 📂 Project Structure (Unity)

- `Assets/Scenes` – Main demo scene(s)  
- `Assets/Scripts` – Core gameplay and interaction scripts  
- `Assets/Prefabs` – Reusable objects used in the environment  
- `ProjectSettings` – Unity project configuration  

Sensitive files like keystore and IDE settings have been removed from the repository for security and cleanliness.

---

## 👤 My Role

This prototype was developed by me as a VR developer.  
I was responsible for:

- Implementing the core color selection and follow mechanic  
- Building the hospital environment flow  
- Setting up the VR interactions and player movement  
- Integrating the logic to randomly choose a color at the start of the session

---
📱 Mobile-Controlled Session Start (Phone → VR Start)

The VR session is started directly from a mobile device.
The person testing the system uses a phone application to:

Start the VR experience remotely,

Select the color that will be used in the experiment,

Trigger the beginning of the tracking session.

Once the color is selected on the phone, the VR application automatically:

Loads the selected color,

Shows the color markers along the path,

Begins tracking the user’s movement and decisions.

This allows easy testing without needing to wear the headset to set parameters.
## ⚠️ Disclaimer

This project was developed for a specific VR use case and is intended **only as a portfolio / demo project**.  
It is not meant for commercial release and does not represent a final product.
