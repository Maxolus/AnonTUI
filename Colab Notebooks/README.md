# Layout Optimization Tool with Optuna

This project performs multi-objective optimization to determine the optimal layout of a modular board system, using [Optuna](https://optuna.org/). It evaluates various configurations of visual, haptic, and vibratory feedback modules in a constrained 5x3 board space.

---

## 💡 Features

- Multi-objective optimization (SART and NASA TLX)
- Layout visualization via `matplotlib`
- Board-space validation with pruning for invalid configurations
- Saves current trial configuration to JSON (for Unity integration)
- Logs all trials with parameters and scores to CSV and SQLite (`study.db`)

---

## 🧩 Modules & Parameters

- 7 modules (e.g., "Orientation at Destination", "Obstacles at Exit")
- Each module can have:
  - Size (Small / Large)
  - Position on a 5x3 board
  - Orientation (Vertical / Horizontal)
  - Color (RGB)
  - Elements Shape (Circle, Triangle, etc.)
  - Haptic Feedback Type (e.g., MX Black, MX Green)
  - Optional Vibration (Intensity, On/Off time)
  - Rotation (on/off)

---

## 🛠 How It Works

1. Loads or creates an Optuna study backed by SQLite (`study.db`).
2. For each trial:
   - Suggests the next module configuration based on Optuna's optimization algorithm (TPE sampler), taking into account previous trial results.
   - Prunes any overlapping or out-of-bounds configurations.
   - Plots the layout visually.
   - Prompts the user to enter **SART** and **NASA TLX** scores.
   - Saves results to:
     - `trial_log.csv`
     - `study.db`
     - `current_trial.json` (in Unity project's Assets folder)

---

## 🚀 Running the Script

### 🔧 Requirements

- Python 3.8+
- `optuna`
- `matplotlib`

Install dependencies:

```bash
pip install optuna matplotlib
```

### ▶️ Run

```bash
python your_script_name.py
```

Follow the on-screen instructions to evaluate each trial.

---

## 📁 Output Files

- `trial_log.csv` — all trials with parameters and scores.
- `current_trial.json` — configuration of the most recent trial (used by Unity).
- `study.db` — SQLite database storing all trial metadata.

---

## 📊 Visualization

Each layout is visualized with numbered rectangles showing position, size, and color of each module.

---

## ✏️ Evaluation Prompts

After each layout, the user is prompted to enter:

- **SART Score** (0–100)
- **NASA TLX Score** (0–100)

These scores are used as objectives for optimization.

---

## 📦 Integration

The saved JSON (`current_trial.json`) can be directly read by Unity for simulation or testing.

---

## 🧪 Number of Trials

By default, the script runs **10 evaluations**. Modify `NUM_EVALS` in the script to increase/decrease.

---

## ❗ Notes

- Prunes invalid configurations early for efficiency.
- Automatically resumes an existing study if one exists.
- Saves results incrementally after each trial.

---

## 📍 Author

Julian Zähnle  
Master Thesis Project  
