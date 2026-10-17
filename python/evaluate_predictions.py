
from pathlib import Path

import pandas as pd

from sklearn.metrics import (
    accuracy_score,
    precision_score,
    recall_score,
    f1_score,
    confusion_matrix,
)


SCENARIOS = {
    "clean": "Clean FedAvg",
    "sign_flipping": "Sign Flipping",
    "gaussian_noise": "Gaussian Noise",
    "model_scaling": "Model Scaling",
}


def evaluate_scenario(dataset_dir, scenario_name, display_name):
    updates_path = dataset_dir / f"{scenario_name}_updates.csv"
    predictions_path = dataset_dir / f"{scenario_name}_predictions.csv"

    updates = pd.read_csv(updates_path)
    predictions = pd.read_csv(predictions_path)

    results = pd.merge(
        updates,
        predictions,
        on=["SimulationId", "UavId", "Round"],
        how="inner",
        validate="one_to_one",
    )

    if len(results) != len(updates) or len(results) != len(predictions):
        raise ValueError(f"Unmatched records in {display_name}")

    actual = results["IsMalicious"]
    predicted = results["PredictedMalicious"]

    tn, fp, fn, tp = confusion_matrix(
        actual,
        predicted,
        labels=[0, 1],
    ).ravel()

    return {
        "Scenario": display_name,
        "Updates": len(results),
        "ActualMalicious": int(actual.sum()),
        "PredictedMalicious": int(predicted.sum()),
        "TP": int(tp),
        "TN": int(tn),
        "FP": int(fp),
        "FN": int(fn),
        "Accuracy": accuracy_score(actual, predicted),
        "Precision": precision_score(
            actual, predicted, zero_division=0
        ),
        "Recall": recall_score(
            actual, predicted, zero_division=0
        ),
        "F1": f1_score(
            actual, predicted, zero_division=0
        ),
    }


def main():
    project_root = Path(__file__).resolve().parent.parent
    dataset_dir = project_root / "datasets"

    evaluations = []

    for scenario_name, display_name in SCENARIOS.items():
        evaluation = evaluate_scenario(
            dataset_dir,
            scenario_name,
            display_name,
        )
        evaluations.append(evaluation)

    summary = pd.DataFrame(evaluations)

    print()
    print("FEDERATED POISONING DETECTOR")
    print("COMPARATIVE EVALUATION")
    print("----------------------------")

    print(
        summary.to_string(
            index=False,
            formatters={
                "Accuracy": "{:.2%}".format,
                "Precision": "{:.2%}".format,
                "Recall": "{:.2%}".format,
                "F1": "{:.2%}".format,
            },
        )
    )

    output_path = dataset_dir / "evaluation_summary.csv"
    summary.to_csv(output_path, index=False)

    print()
    print(f"Evaluation saved to: {output_path}")


if __name__ == "__main__":
    main()
