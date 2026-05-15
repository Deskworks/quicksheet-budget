# quicksheet-budget

Budget envelope visualizer for [QuickSheet](https://github.com/cemheren/QuickSheet) — track spending categories with visual progress bars right on your desktop wallpaper.

## Install

In any QuickSheet cell:
```
ext: github:cemheren/quicksheet-budget
```

## Usage

```
budget: CategoryName, budgetAmount, spentAmount
```

### Examples

| Cell | What it shows |
|------|--------------|
| `budget: Groceries, 800, 623` | 🟡 Groceries 77.9% [████████████████░░░░] |
| `budget: Software, 200, 89` | 🟢 Software 44.5% [█████████░░░░░░░░░░░] |
| `budget: Rent, 2000, 2000` | 🔴 Rent 100.0% [████████████████████] |
| `budget: Fun, 300, 450` | 🔴 Fun 150.0% [████████████████████] ⚠️ $150 over! |

### Output (3 rows × 2 columns)

```
🟡 Groceries          77.9%  [████████████████░░░░]
Spent: $623.00        Budget: $800.00
✅ $177.00 remaining
```

## Dashboard Recipe

Build a full budget dashboard by referencing cell values:

```csv
Category,Budget,Spent,Tracker
Groceries,800,623,"budget: Groceries, {B2}, {C2}"
Software,200,89,"budget: Software, {B3}, {C3}"
Dining,400,387,"budget: Dining, {B4}, {C4}"
Transport,150,92,"budget: Transport, {B5}, {C5}"
```

Update the "Spent" column → the progress bars update live on your desktop.

## Color Coding

| % Used | Indicator |
|--------|-----------|
| ≤ 50%  | 🟢 On track |
| ≤ 75%  | 🟡 Watch it |
| ≤ 90%  | 🟠 Getting close |
| > 90%  | 🔴 Over/near limit |

## Requirements

- .NET 9 SDK
- QuickSheet (any mode)

## License

MIT
