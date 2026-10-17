
from pathlib import Path
import pandas as pd


def main():
    project_root = Path(__file__).resolve().parent.parent
    csv_path = project_root / "datasets" / "training.csv"

    df = pd.read_csv(csv_path)

    print("FEDERATED POISONING DETECTOR")
    print("----------------------------")
    print()

    print("Dataset dimensions:")
    print(df.shape)
    print()

    print("Dataset columns:")
    print(df.columns.tolist())
    print()

    print("First five records:")
    print(df.head())
    print()

    print("Class distribution:")
    print(df["IsMalicious"].value_counts().sort_index())
    print()

    print("Number of simulations:")
    print(df["SimulationId"].nunique())
    print()

    print("Missing values:")
    print(df.isnull().sum())


if __name__ == "__main__":
    main()
