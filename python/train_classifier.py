
from pathlib import Path

import joblib
import pandas as pd

from sklearn.ensemble import RandomForestClassifier
from sklearn.metrics import (
    accuracy_score,
    classification_report,
    confusion_matrix,
)
from sklearn.model_selection import GroupShuffleSplit


FEATURE_COLUMNS = [
    "WeightDelta",
    "BiasDelta",
    "UpdateMagnitude",
    "SampleCount",
]


def main():
    project_root = Path(__file__).resolve().parent.parent

    dataset_path = project_root / "datasets" / "training.csv"
    model_path = project_root / "python" / "poisoning_detector.joblib"

    df = pd.read_csv(dataset_path)

    X = df[FEATURE_COLUMNS]
    y = df["IsMalicious"]
    groups = df["SimulationId"]

    splitter = GroupShuffleSplit(
        n_splits=1,
        test_size=0.20,
        random_state=42,
    )

    train_indices, test_indices = next(
        splitter.split(X, y, groups=groups)
    )

    X_train = X.iloc[train_indices]
    X_test = X.iloc[test_indices]

    y_train = y.iloc[train_indices]
    y_test = y.iloc[test_indices]

    classifier = RandomForestClassifier(
        n_estimators=100,
        random_state=42,
        class_weight="balanced",
    )

    classifier.fit(X_train, y_train)

    predictions = classifier.predict(X_test)

    print("RANDOM FOREST CLASSIFIER")
    print("------------------------")

    print(f"Training records: {len(X_train)}")
    print(f"Testing records : {len(X_test)}")

    print()
    print(f"Accuracy: {accuracy_score(y_test, predictions):.2%}")

    print()
    print("Confusion matrix:")
    print(confusion_matrix(y_test, predictions))

    print()
    print("Classification report:")
    print(classification_report(
        y_test,
        predictions,
        target_names=["Legitimate", "Malicious"],
        zero_division=0,
    ))

    joblib.dump(
        {
            "model": classifier,
            "features": FEATURE_COLUMNS,
        },
        model_path,
    )

    print()
    print(f"Model saved to: {model_path}")


if __name__ == "__main__":
    main()
