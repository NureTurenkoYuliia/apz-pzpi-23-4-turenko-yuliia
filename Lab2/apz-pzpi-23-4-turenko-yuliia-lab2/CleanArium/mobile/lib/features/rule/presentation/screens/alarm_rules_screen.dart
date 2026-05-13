import 'package:flutter/material.dart';
import '../../../../core/utils/token_storage.dart';
import '../../../../core/network/api_client.dart';
import '../../data/datasources/alarm_rule_api.dart';
import '../../../../shared/widgets/language_button.dart';
import 'package:easy_localization/easy_localization.dart';

class AlarmRulesScreen extends StatefulWidget {
  final int deviceId;

  const AlarmRulesScreen({super.key, required this.deviceId});

  @override
  State<AlarmRulesScreen> createState() => _AlarmRulesScreenState();
}

class _AlarmRulesScreenState extends State<AlarmRulesScreen> {
  List<dynamic> rules = [];
  bool isLoading = true;

  late AlarmRuleApi api;

  @override
  void initState() {
    super.initState();
    init();
  }

  Future<void> init() async {
    final token = await TokenStorage().getAccessToken();
    final client = ApiClient(token).dio;

    api = AlarmRuleApi(client);

    await load();
  }

  Future<void> load() async {
    setState(() => isLoading = true);

    try {
      rules = await api.getByDevice(widget.deviceId);
    } catch (e) {
      debugPrint(e.toString());
    }

    if (!mounted) return;

    setState(() => isLoading = false);
  }

  String conditionToText(int value) {
    switch (value) {
      case 1:
        return "condition_less".tr();
      case 2:
        return "condition_greater".tr();
      case 3:
        return "condition_less_equal".tr();
      case 4:
        return "condition_greater_equal".tr();
      case 5:
        return "condition_equal".tr();
      case 6:
        return "condition_not_equal".tr();
      default:
        return "-";
    }
  }

  void createRule() async {
    int? condition;

    final thresholdController = TextEditingController();
    final unitController = TextEditingController();

    await showDialog(
      context: context,
      builder: (_) => StatefulBuilder(
        builder: (context, setStateDialog) {
          return AlertDialog(
            title: Text("create_rule".tr()),
            content: SingleChildScrollView(
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  DropdownButtonFormField<int>(
                    decoration: InputDecoration(
                      labelText: "condition".tr(),
                    ),
                    items: [
                      DropdownMenuItem(
                        value: 1,
                        child: Text("condition_less".tr()),
                      ),
                      DropdownMenuItem(
                        value: 2,
                        child: Text("condition_greater".tr()),
                      ),
                      DropdownMenuItem(
                        value: 3,
                        child: Text("condition_less_equal".tr()),
                      ),
                      DropdownMenuItem(
                        value: 4,
                        child: Text("condition_greater_equal".tr()),
                      ),
                      DropdownMenuItem(
                        value: 5,
                        child: Text("condition_equal".tr()),
                      ),
                      DropdownMenuItem(
                        value: 6,
                        child: Text("condition_not_equal".tr()),
                      ),
                    ],
                    onChanged: (v) {
                      condition = v;
                    },
                  ),

                  const SizedBox(height: 12),

                  TextField(
                    controller: thresholdController,
                    keyboardType: TextInputType.number,
                    decoration: InputDecoration(
                      labelText: "threshold".tr(),
                    ),
                  ),

                  const SizedBox(height: 12),

                  TextField(
                    controller: unitController,
                    decoration: InputDecoration(
                      labelText: "unit".tr(),
                    ),
                  ),
                ],
              ),
            ),
            actions: [
              ElevatedButton(
                onPressed: () async {
                  if (condition == null ||
                      thresholdController.text.isEmpty ||
                      unitController.text.isEmpty) {
                    return;
                  }

                  await api.create(
                    widget.deviceId,
                    condition!,
                    double.parse(thresholdController.text),
                    unitController.text.trim(),
                  );

                  if (!mounted) return;

                  Navigator.pop(context);

                  load();
                },
                child: Text("create".tr()),
              ),
            ],
          );
        },
      ),
    );
  }

  void editRule(dynamic rule) async {
    int condition = rule.condition;

    final thresholdController = TextEditingController(
      text: rule.threshold.toString(),
    );

    final unitController = TextEditingController(
      text: rule.unit,
    );

    bool isActive = rule.isActive;

    await showDialog(
      context: context,
      builder: (_) => StatefulBuilder(
        builder: (context, setStateDialog) {
          return AlertDialog(
            title: Text("edit_rule".tr()),
            content: SingleChildScrollView(
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  DropdownButtonFormField<int>(
                    initialValue: condition,
                    decoration: InputDecoration(
                      labelText: "condition".tr(),
                    ),
                    items: [
                      DropdownMenuItem(
                        value: 1,
                        child: Text("condition_less".tr()),
                      ),
                      DropdownMenuItem(
                        value: 2,
                        child: Text("condition_greater".tr()),
                      ),
                      DropdownMenuItem(
                        value: 3,
                        child: Text("condition_less_equal".tr()),
                      ),
                      DropdownMenuItem(
                        value: 4,
                        child: Text("condition_greater_equal".tr()),
                      ),
                      DropdownMenuItem(
                        value: 5,
                        child: Text("condition_equal".tr()),
                      ),
                      DropdownMenuItem(
                        value: 6,
                        child: Text("condition_not_equal".tr()),
                      ),
                    ],
                    onChanged: (v) {
                      setStateDialog(() {
                        condition = v!;
                      });
                    },
                  ),

                  const SizedBox(height: 12),

                  TextField(
                    controller: thresholdController,
                    keyboardType: TextInputType.number,
                    decoration: InputDecoration(
                      labelText: "threshold".tr(),
                    ),
                  ),

                  const SizedBox(height: 12),

                  TextField(
                    controller: unitController,
                    decoration: InputDecoration(
                      labelText: "unit".tr(),
                    ),
                  ),

                  const SizedBox(height: 12),

                  SwitchListTile(
                    value: isActive,
                    onChanged: (v) {
                      setStateDialog(() {
                        isActive = v;
                      });
                    },
                    title: Text("active".tr()),
                  ),
                ],
              ),
            ),
            actions: [
              ElevatedButton(
                onPressed: () async {
                  await api.update(
                    rule.id,
                    condition,
                    double.parse(thresholdController.text),
                    unitController.text.trim(),
                    isActive,
                  );

                  if (!mounted) return;

                  Navigator.pop(context);

                  load();
                },
                child: Text("save".tr()),
              ),
            ],
          );
        },
      ),
    );
  }

  void deleteRule(int id) async {
    await api.delete(id);

    load();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("label_alarm_rules".tr()),
        actions: [
          LanguageButton(
            onChanged: () {
              setState(() {});
            },
          ),
        ],
      ),

      floatingActionButton: FloatingActionButton(
        onPressed: createRule,
        child: const Icon(Icons.add),
      ),

      body: isLoading
          ? const Center(
              child: CircularProgressIndicator(),
            )
          : rules.isEmpty
              ? Center(
                  child: Text("no_rules".tr()),
                )
              : ListView.builder(
                  itemCount: rules.length,
                  itemBuilder: (_, i) {
                    final r = rules[i];

                    return Card(
                      margin: const EdgeInsets.symmetric(
                        horizontal: 12,
                        vertical: 6,
                      ),
                      child: ListTile(
                        title: Text(
                          "${"threshold".tr()}: ${r.threshold} ${r.unit}",
                        ),

                        subtitle: Text(
                          "${"condition".tr()}: ${conditionToText(r.condition)} | "
                          "${"active".tr()}: ${r.isActive}",
                        ),

                        trailing: Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            IconButton(
                              icon: const Icon(Icons.edit),
                              onPressed: () => editRule(r),
                            ),

                            IconButton(
                              icon: const Icon(Icons.delete),
                              onPressed: () => deleteRule(r.id),
                            ),
                          ],
                        ),
                      ),
                    );
                  },
                ),
    );
  }
}