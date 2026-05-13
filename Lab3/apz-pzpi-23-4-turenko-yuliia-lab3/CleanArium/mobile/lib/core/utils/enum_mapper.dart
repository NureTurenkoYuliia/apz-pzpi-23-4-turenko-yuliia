String conditionToText(int value) {
  switch (value) {
    case 1: return "<";
    case 2: return ">";
    case 3: return "<=";
    case 4: return ">=";
    case 5: return "=";
    case 6: return "!=";
    default: return "Unknown";
  }
}

String commandTypeToText(int value) {
  switch (value) {
    case 1: return "Turn On";
    case 2: return "Turn Off";
    case 3: return "Set Value";
    case 4: return "Calibrate";
    default: return "Unknown";
  }
}

String repeatModeToText(int value) {
  switch (value) {
    case 1: return "None";
    case 2: return "Daily";
    case 3: return "Weekly";
    case 4: return "Interval";
    default: return "Unknown";
  }
}