# MoTUI: Personalization of In-Vehicle Tactile Interfaces for People With Vision Impairments and the Blind

<p align="center">
  <img src="teaser.png" alt="MoTUI Teaser Figure" width="100%">
</p>

<p align="center">
  <a href="https://doi.org/XXXXXXX.XXXXXXX"><img src="https://img.shields.io/badge/DOI-10.1145%2FXXXXXXX.XXXXXXX-blue?logo=doi" alt="DOI"></a>
  <a href="https://arxiv.org/abs/XXXX.XXXXX"><img src="https://img.shields.io/badge/arXiv-XXXX.XXXXX-b31b1b?logo=arxiv" alt="arXiv"></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-yellow.svg" alt="License: MIT"></a>
  <a href="#"><img src="https://img.shields.io/badge/Open%20Source-Hardware%20%2B%20Software-orange?logo=opensourceinitiative" alt="Open Source"></a>
  <a href="#"><img src="https://img.shields.io/badge/Python-3.12-3776AB?logo=python&logoColor=white" alt="Python 3.12"></a>
  <a href="#"><img src="https://img.shields.io/badge/Unity-6000.0.53f1%20LTS-000000?logo=unity&logoColor=white" alt="Unity"></a>
  <a href="#"><img src="https://img.shields.io/badge/Arduino-Mega%202560-00979D?logo=arduino&logoColor=white" alt="Arduino"></a>
  <a href="#"><img src="https://img.shields.io/badge/Optuna-4.5.0-1674b1?logo=python&logoColor=white" alt="Optuna"></a>
  <a href="#"><img src="https://img.shields.io/badge/Accessibility-♿%20VIPs-purple" alt="Accessibility"></a>
</p>

---

## 📄 About

**MoTUI** is an open-source **Modular Tactile User Interface** that pairs reconfigurable hardware with **Human-in-the-Loop Multi-Objective Bayesian Optimization (HITL-MOBO)** to personalize in-vehicle interfaces for people with vision impairments (VIPs). Instead of one-size-fits-all designs, MoTUI lets each user arrive at their own Pareto-optimal configuration through iterative feedback.

> *"The last presentation of the 21 runs, that last demonstration, that was mine: the speech rate was right, the elements were perfectly tangible, and the cues like 'stop because of pedestrian' or 'oncoming car' and the mention of obstacles at the exit all fit. With that setup I could imagine getting into such a car and driving off alone."* — P9

## ✨ Key Results

In a within-subject study (**N=12 VIPs**), HITL-optimized configurations significantly outperformed both a static baseline and an LLM-generated design:

| Metric | Optimization | LLM Baseline | Static (OnBoard) | Sig. |
|---|---|---|---|---|
| **Situation Awareness** | M=0.85, SD=0.11 | M=0.57, SD=0.33 | M=0.74, SD=0.22 | ✅ Opt > LLM* |
| **RAW NASA-TLX** (↓ better) | M=0.14, SD=0.09 | M=0.36, SD=0.24 | M=0.23, SD=0.12 | ✅ Opt < LLM* |
| **Look & Feel** | M=0.82, SD=0.11 | M=0.57, SD=0.29 | M=0.71, SD=0.27 | ✅ Opt > LLM* |
| **Ownership** | M=5.83, SD=1.34 | M=4.17, SD=1.95 | M=4.58, SD=1.73 | ✅ Opt > LLM* & Static* |

> \* p < .05

Crucially, the optimization converged on **substantially different configurations** across participants — confirming that personalization is essential and cannot be replaced by group-level defaults.

## 🧩 System Overview

### Hardware
Each MoTUI module features:
- 🔘 **MX Button** with tactile response (Linear / Tactile / Clicky)
- 🔄 **360° Rotation** (1 DOF)
- ⬆️ **Vertical rising movement** (1 DOF)
- ↔️ **Horizontal slide** (1 DOF)
- 📳 **Vibration motor**
- 💡 **RGB LED**
- 🔲 **Interchangeable top elements** (Circle, Triangle, Square, Hexagon, Arrow)

Modules are housed in laser-cut wooden chassis (88×88mm or 88×176mm) and arranged on a **5×3 base plate** (47×30cm) offering up to **15 positions**. Connected to an **Arduino Mega 2560** via custom connectors with **6× PCA9685 PWM driver boards**.

### Seven Information Needs (1 per module)

| # | Information Need | Rotation | Vertical | Horizontal | Speech Example |
|---|---|---|---|---|---|
| 0 | 🧭 Orientation to destination | Points to destination | Extends at arrival | Reflects destination position | *"Your final destination is to the right behind you."* |
| 1 | 🚪 Exit direction | Points to exit | Extends at arrival | Shifts per exit side | *"Safe exit direction on the right."* |
| 2 | ⚠️ Obstacles at exit | Points to obstacle | Extends if obstacle | Reflects obstacle position | *"Caution: a tree to your right in front."* |
| 3 | ⏱️ Time to destination | Clock-like movement | Retracts at arrival | Progress bar | *"45 seconds left to reach destination."* |
| 4 | 🛑 Reason for stopping | Points to reason | Extends at stop | Progress bar for duration | *"Reason for stop: pedestrian crossing."* |
| 5 | 🚗 Other traffic | Points to traffic | Extends when near | Reflects position | *"Oncoming traffic on the left side."* |
| 6 | ↩️ Future turns | Points per turn | Extends if upcoming | Distance indicator | *"Upcoming left turn."* |

### Optimization Pipeline

```
┌─────────────────────────────────────────────────────┐
│  HITL-MOBO with MOTPE (Optuna 4.5.0)               │
│                                                     │
│  15 Sampling Trials  →  5 Optimization Trials       │
│  (fixed seed, all      (MOTPE refines based         │
│   modules appear ≥5×)   on participant ratings)     │
│                                                     │
│  Objectives:                                        │
│    ↑ Maximize Situation Awareness                   │
│    ↑ Maximize Look & Feel                           │
│    ↓ Minimize Perceived Workload (NASA-TLX)         │
│                                                     │
│  → Pareto-optimal non-dominant solution per user    │
└─────────────────────────────────────────────────────┘
```

### Parameter Space

**Global Parameters** (shared across modules):
- Color hue (HSV, S=V=100%)
- Button type (MX Black / Grey / Green)
- Vibration intensity, on-time, off-time
- Servo speed (100–300 deg/s)
- Speech output speed (100–250 WPM)

**Local Parameters** (per module, conditional on activation):
- x position (0–4), y position (0–2)
- Vertical movement, top element shape
- Use vibration, use rotation

## 🛠️ Repository Structure

```
MoTUI/
├── hardware/               # CAD files, circuit plans, manufacturing guide
│   ├── cad/                # 3D models for module chassis
│   ├── circuits/           # Arduino schematics & PCB designs
│   └── bom.csv             # Bill of materials
├── firmware/               # Arduino Mega 2560 firmware
├── unity/                  # Unity simulation environment (6000.0.53f1 LTS)
│   ├── Scenes/             # Urban HAV ride simulation
│   └── Scripts/            # Event triggers, module bridge
├── optimizer/              # Python HITL-MOBO pipeline
│   ├── motpe_optimizer.py  # Optuna MOTPE integration
│   ├── bridge.py           # Unity ↔ Arduino ↔ Optimizer communication
│   └── analysis/           # R scripts for statistical analysis
├── configs/                # Example Pareto-optimal configurations (N=12)
├── data/                   # Anonymized study data
└── README.md
```

## 🚀 Getting Started

### Prerequisites

- **Python 3.12+** with [Optuna](https://optuna.org/) 4.5.0
- **Unity 6000.0.53f1 LTS**
- **Arduino IDE** (for Mega 2560 firmware)
- **R 4.5.1** (for analysis scripts)

### Installation

```bash
# Clone the repository
git clone https://github.com/Maxolus/AnonTUI.git
cd AnonTUI

# Install Python dependencies
pip install -r requirements.txt

# Flash Arduino firmware
# (see hardware/README.md for wiring guide)

# Open Unity project
# (see unity/README.md for simulation setup)
```

### Running the Optimizer

```bash
python optimizer/motpe_optimizer.py \
  --n_sampling 15 \
  --n_optimization 5 \
  --seed 42
```

## 📊 Study Design

- **N=12** participants with low vision or legal blindness (M age = 56.75, SD = 15.47)
- **Within-subject**, counterbalanced across 3 conditions:
  1. **HITL-MOTPE Optimization** (20 trials: 15 sampling + 5 optimization)
  2. **OnBoard-inspired static baseline** ([Meinhardt et al., 2024](https://doi.org/10.1145/3659618))
  3. **LLM-generated configuration** (ChatGPT-5.2, single-shot from vision test data)
- **Simulation**: 40s urban HAV ride on **YawVR 2** motion chair with 75" 4K display
- **Measures**: Situation Awareness (SART), NASA-TLX, Look & Feel, Psychological Ownership

## 🔬 Contributions

- **Artifact Contribution**: MoTUI — an open-source tactile UI system and connected optimization framework for recreating and personalizing in-vehicle interface designs.
- **Empirical Study about People**: A within-subject study (N=12 VIPs) demonstrating how HITL-MOBO improves situation awareness, lowers mental workload, increases look & feel, and highlights the necessity of personalization over one-size-fits-all approaches.

## 📚 Citation

```bibtex
@inproceedings{anon2026motui,
  title     = {MoTUI: Personalization of In-Vehicle Tactile Interfaces 
               for People With Vision Impairments and the Blind},
  author    = {Anonymous Author(s)},
  booktitle = {Proceedings of the ACM Symposium on User Interface 
               Software and Technology (UIST '26)},
  year      = {2026},
  publisher = {ACM},
  doi       = {XXXXXXX.XXXXXXX}
}
```

## 🙏 Acknowledgments

We thank all 12 participants for their time, trust, and invaluable feedback during the study sessions.

## 📜 License

This project is open-sourced under the [MIT License](LICENSE).

---

<p align="center">
  <i>MoTUI enables personalized spatial tactile designs — because accessibility is not one-size-fits-all.</i>
</p>
