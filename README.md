# Federated Poisoning Detector

**Design and Implementation of a Python/C#-Based Machine Learning Pipeline for Detecting Malicious Model Updates in Federated Learning**

A research demonstrator investigating the detection of malicious model updates in federated learning using a C# simulation framework and a Python machine learning classifier.

## 1. Project Overview

Federated learning enables multiple participants to train a shared machine learning model without directly sharing their local training datasets.

However, malicious or compromised participants may submit manipulated model updates, potentially degrading the performance of the global model.

This project investigates whether a supervised machine learning classifier can distinguish legitimate model updates from deliberately poisoned updates.

The application simulates a small UAV swarm performing federated learning and evaluates three model-poisoning strategies.

## 2. Project Objectives

- Simulate federated learning across multiple UAVs using C#.
- Implement conventional Federated Averaging (FedAvg).
- Generate legitimate and malicious model updates.
- Implement sign-flipping, Gaussian-noise and model-scaling attacks.
- Generate a labelled dataset for supervised machine learning.
- Train a Random Forest classifier using Python and scikit-learn.
- Integrate the Python classifier with the C# application.
- Evaluate detection performance using standard classification metrics.

## 3. System Architecture

The application consists of two principal components.

**C#/.NET 8 — Federated Learning Simulation**

The C# application simulates five UAVs, each maintaining local training data. The UAVs train local logistic regression models and submit their model updates to a federated server.

The server performs FedAvg aggregation across ten training rounds.

One UAV can be configured to submit malicious model updates using a selected poisoning strategy.

The simulation exports model-update features and ground-truth labels to CSV files.

**Python — Machine Learning Detection**

Python uses pandas and scikit-learn to process the generated dataset.

A Random Forest classifier is trained using four features:

- WeightDelta
- BiasDelta
- UpdateMagnitude
- SampleCount

The trained classifier predicts whether submitted updates are legitimate or malicious.

Prediction results are exported to CSV and read by the C# application.

The current system performs detection and reporting after aggregation; it does not block malicious updates before they influence the global model.

## 4. Technologies

| Component | Technology |
|---|---|
| Simulation | C# / .NET 8 |
| Development environment | Visual Studio 2022 |
| Machine learning | Python / scikit-learn |
| Data processing | pandas |
| Model persistence | joblib |
| Federated aggregation | FedAvg |
| Local learning algorithm | Logistic regression |
| Detection algorithm | Random Forest |
| Data interchange | CSV |

## 5. Experimental Scenarios

Four scenarios are supported:

1. **Clean FedAvg:** All UAVs submit legitimate model updates.
2. **Sign-Flipping Attack:** A compromised UAV reverses and amplifies its model update.
3. **Gaussian-Noise Attack:** Random perturbations are introduced into a compromised UAV's model update.
4. **Model-Scaling Attack:** A compromised UAV amplifies its model update.

The demonstration uses five UAVs and ten federated training rounds per scenario.

## 6. Machine Learning Dataset

The C# dataset generator produces 200 simulation runs containing 10,000 labelled model-update records.

The dataset contains 8,500 legitimate updates and 1,500 malicious updates.

A group-aware training/test split separates simulation runs to prevent updates from the same run appearing in both subsets.

The classifier is trained on 8,000 records and evaluated on 2,000 held-out records.

The held-out evaluation achieved approximately 95.2% accuracy.

These results are based on synthetic data and should not be interpreted as evidence of performance in operational UAV environments.

## 7. Demonstration Results

The integrated C#–Python pipeline was tested against four controlled scenarios, each containing 50 model updates.

| Scenario | Accuracy | Precision | Recall | F1-score |
|---|---:|---:|---:|---:|
| Clean FedAvg | 100% | N/A | N/A | N/A |
| Sign flipping | 96% | 83.33% | 100% | 90.91% |
| Gaussian noise | 94% | 88.89% | 80% | 84.21% |
| Model scaling | 100% | 100% | 100% | 100% |

Across the four demonstrations, 195 of 200 model updates were classified correctly.

The Gaussian-noise demonstration used an attack strength outside the range employed during training-data generation. Consequently, these scenarios should not be treated as a controlled comparison of attack difficulty.

## 8. Installation and Reproduction

The following instructions assume Windows, .NET 8 and a compatible Python installation.

Clone the repository:

```powershell
git clone https://github.com/Thanet-Coder/FederatedPoisoningDetector.git
cd FederatedPoisoningDetector
```

Create and activate a Python virtual environment:

```powershell
python -m venv .venv
.\.venv\Scripts\Activate.ps1
```

Install the Python dependencies:

```powershell
pip install -r requirements.txt
```

Run the C# application:

```powershell
dotnet run
```

Select option **5** to generate the machine learning dataset.

Train the Python classifier:

```powershell
python .\python\train_classifier.py
```

Run the C# application again:

```powershell
dotnet run
```

Select option **6**, followed by the desired detection scenario.

Run all four scenarios to generate their corresponding CSV files.

Finally, generate the comparative evaluation:

```powershell
python .\python\evaluate_predictions.py
```

The resulting summary is saved to `datasets/evaluation_summary.csv`.

Generated datasets and trained model files may be excluded from version control and can be regenerated using the instructions above.

## 9. Limitations

The project is a proof-of-concept research demonstrator.

Its principal limitations include:

- Synthetic rather than real UAV sensor data.
- A small five-UAV simulation.
- Limited poisoning strategies.
- Shared underlying data-generation patterns.
- No evaluation of privacy leakage or communication overhead.
- No measurement of operational deployment costs.
- Detection occurs after aggregation rather than preventing poisoning.
- Limited evidence of generalisation to unseen data distributions or attack strategies.

## 10. Future Work

Potential extensions include:

- Detection-assisted or trust-aware aggregation.
- Evaluation against stealthier poisoning and backdoor attacks.
- Larger and more heterogeneous UAV swarms.
- Non-IID data and unreliable communication experiments.
- Evaluation on independent datasets.
- More extensive repeated trials and statistical analysis.
- Measurement of computational and communication overhead.

## 11. Research Context

This project was independently developed as a small research demonstrator exploring machine learning approaches to detecting malicious model updates in federated learning.

It is intended as a foundation for further investigation into trustworthy and attack-resilient federated learning.

## 12. Repository

Source code:

https://github.com/Thanet-Coder/FederatedPoisoningDetector

**Status:** Functional research prototype with integrated C# simulation, Python classification and comparative evaluation.