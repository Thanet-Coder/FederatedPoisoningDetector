
from pathlib import Path

import pandas as pd
from sklearn.model_selection import GroupShuffleSplit


FEATURE_COLUMNS = [
    "WeightDelta",
    "BiasDelta",
    "UpdateMagnitude",
    "SampleCount",
]


def main():
    project_root = Path(__file__).resolve().parent.parent
    csv_path = project_root / "datasets" / "training.csv"

    df = pd.read_csv(csv_path)

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

    train_simulations = set(groups.iloc[train_indices])
    test_simulations = set(groups.iloc[test_indices])

    print("TRAINING AND TESTING DATA")
    print("-------------------------")

    print(f"Training records: {len(X_train)}")
    print(f"Testing records : {len(X_test)}")

    print(f"Training simulations: {len(train_simulations)}")
    print(f"Testing simulations : {len(test_simulations)}")

    print()
    print("Training class distribution:")
    print(y_train.value_counts().sort_index())

    print()
    print("Testing class distribution:")
    print(y_test.value_counts().sort_index())

    print()
    print(
        "Simulation overlap:",
        len(train_simulations.intersection(test_simulations)),
    )


if __name__ == "__main__":
    main()

