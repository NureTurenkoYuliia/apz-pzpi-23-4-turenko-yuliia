import 'package:flutter/material.dart';
import '../../../../core/utils/token_storage.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/utils/enum_mapper.dart';
import '../../data/datasources/command_api.dart';
import '../../../../shared/widgets/language_button.dart';
import 'package:easy_localization/easy_localization.dart';

class CommandsScreen extends StatefulWidget {
  final int deviceId;

  const CommandsScreen({super.key, required this.deviceId});

  @override
  State<CommandsScreen> createState() => _CommandsScreenState();
}

class _CommandsScreenState extends State<CommandsScreen> {
  List<dynamic> commands = [];
  bool isLoading = true;

  late CommandApi api;

  @override
  void initState() {
    super.initState();
    init();
  }

  Future<void> init() async {
    final token = await TokenStorage().getAccessToken();
    final client = ApiClient(token).dio;

    api = CommandApi(client);

    load();
  }

  Future<void> load() async {
    setState(() => isLoading = true);

    try {
      commands = await api.getByDevice(widget.deviceId);
    } catch (e) {
      debugPrint(e.toString());
    }

    setState(() => isLoading = false);
  }

  void sendCommand(int type) async {
    await api.executeNow(widget.deviceId, type);
  }

  void createCommand() async {
    int? type;
    int repeat = 1;
    bool isActive = true;
    int? intervalMinutes;

    DateTime startTime = DateTime.now();

    final intervalController = TextEditingController();

    await showDialog(
      context: context,
      builder: (_) => StatefulBuilder(
        builder: (context, setStateDialog) {
          return AlertDialog(
            title: Text("command_create_title".tr()),
            content: SingleChildScrollView(
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  DropdownButtonFormField<int>(
                    decoration: InputDecoration(labelText: "command_type".tr()),
                    items: [
                      DropdownMenuItem(
                        value: 1,
                        child: Text("command_action_on".tr()),
                      ),
                      DropdownMenuItem(
                        value: 2,
                        child: Text("command_action_off".tr()),
                      ),
                      DropdownMenuItem(
                        value: 3,
                        child: Text("command_action_set_value".tr()),
                      ),
                      DropdownMenuItem(
                        value: 4,
                        child: Text("command_action_calibrate".tr()),
                      ),
                    ],
                    onChanged: (v) => type = v,
                  ),

                  const SizedBox(height: 12),

                  ListTile(
                    contentPadding: EdgeInsets.zero,
                    title: Text("command_start_time".tr()),
                    subtitle: Text(
                      "${startTime.day}.${startTime.month}.${startTime.year} "
                      "${startTime.hour}:${startTime.minute.toString().padLeft(2, '0')}",
                    ),
                    trailing: const Icon(Icons.access_time),
                    onTap: () async {
                      final pickedDate = await showDatePicker(
                        context: context,
                        initialDate: startTime,
                        firstDate: DateTime.now(),
                        lastDate: DateTime(2100),
                      );

                      if (pickedDate == null) return;

                      final pickedTime = await showTimePicker(
                        context: context,
                        initialTime: TimeOfDay.fromDateTime(startTime),
                      );

                      if (pickedTime == null) return;

                      setStateDialog(() {
                        startTime = DateTime(
                          pickedDate.year,
                          pickedDate.month,
                          pickedDate.day,
                          pickedTime.hour,
                          pickedTime.minute,
                        );
                      });
                    },
                  ),

                  const SizedBox(height: 12),

                  DropdownButtonFormField<int>(
                    initialValue: repeat,
                    decoration: InputDecoration(
                      labelText: "repeat_mode_label".tr(),
                    ),
                    items: [
                      DropdownMenuItem(
                        value: 1,
                        child: Text("repeat_none".tr()),
                      ),
                      DropdownMenuItem(
                        value: 2,
                        child: Text("repeat_daily".tr()),
                      ),
                      DropdownMenuItem(
                        value: 3,
                        child: Text("repeat_weekly".tr()),
                      ),
                      DropdownMenuItem(
                        value: 4,
                        child: Text("repeat_interval".tr()),
                      ),
                    ],
                    onChanged: (v) {
                      setStateDialog(() {
                        repeat = v!;
                      });
                    },
                  ),

                  if (repeat == 4) ...[
                    const SizedBox(height: 12),

                    TextFormField(
                      controller: intervalController,
                      keyboardType: TextInputType.number,
                      decoration: InputDecoration(
                        labelText: "repeat_interval_minutes".tr(),
                      ),
                    ),
                  ],

                  const SizedBox(height: 12),

                  SwitchListTile(
                    value: isActive,
                    onChanged: (v) {
                      setStateDialog(() {
                        isActive = v;
                      });
                    },
                    title: Text("state_active".tr()),
                  ),
                ],
              ),
            ),
            actions: [
              ElevatedButton(
                onPressed: () async {
                  if (type == null) return;

                  intervalMinutes = repeat == 4
                      ? int.tryParse(intervalController.text)
                      : null;

                  await api.create(
                    widget.deviceId,
                    type!,
                    startTime,
                    repeat,
                    intervalMinutes,
                    isActive,
                  );

                  if (!mounted) return;

                  Navigator.pop(context);
                  load();
                },
                child: Text("action_create".tr()),
              ),
            ],
          );
        },
      ),
    );
  }

  void editCommand(dynamic c) async {
    int type = c.commandType;
    int repeat = c.repeatMode;
    bool isActive = c.isActive;

    DateTime startTime = DateTime.parse(c.startTime.toString());

    final intervalController = TextEditingController(
      text: c.intervalMinutes?.toString() ?? '',
    );

    await showDialog(
      context: context,
      builder: (_) => StatefulBuilder(
        builder: (context, setStateDialog) {
          return AlertDialog(
            title: Text("command_edit_title".tr()),
            content: SingleChildScrollView(
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  DropdownButtonFormField<int>(
                    initialValue: type,
                    decoration: InputDecoration(labelText: "command_type".tr()),
                    items: [
                      DropdownMenuItem(
                        value: 1,
                        child: Text("command_action_on".tr()),
                      ),
                      DropdownMenuItem(
                        value: 2,
                        child: Text("command_action_off".tr()),
                      ),
                      DropdownMenuItem(
                        value: 3,
                        child: Text("command_action_set_value".tr()),
                      ),
                      DropdownMenuItem(
                        value: 4,
                        child: Text("command_action_calibrate".tr()),
                      ),
                    ],
                    onChanged: (v) {
                      setStateDialog(() {
                        type = v!;
                      });
                    },
                  ),

                  const SizedBox(height: 12),

                  ListTile(
                    contentPadding: EdgeInsets.zero,
                    title: Text("command_start_time".tr()),
                    subtitle: Text(
                      "${startTime.day}.${startTime.month}.${startTime.year} "
                      "${startTime.hour}:${startTime.minute.toString().padLeft(2, '0')}",
                    ),
                    trailing: const Icon(Icons.access_time),
                    onTap: () async {
                      final pickedDate = await showDatePicker(
                        context: context,
                        initialDate: startTime,
                        firstDate: DateTime.now(),
                        lastDate: DateTime(2100),
                      );

                      if (pickedDate == null) return;

                      final pickedTime = await showTimePicker(
                        context: context,
                        initialTime: TimeOfDay.fromDateTime(startTime),
                      );

                      if (pickedTime == null) return;

                      setStateDialog(() {
                        startTime = DateTime(
                          pickedDate.year,
                          pickedDate.month,
                          pickedDate.day,
                          pickedTime.hour,
                          pickedTime.minute,
                        );
                      });
                    },
                  ),

                  const SizedBox(height: 12),

                  DropdownButtonFormField<int>(
                    initialValue: repeat,
                    decoration: InputDecoration(
                      labelText: "repeat_mode_label".tr(),
                    ),
                    items: [
                      DropdownMenuItem(
                        value: 1,
                        child: Text("repeat_none".tr()),
                      ),
                      DropdownMenuItem(
                        value: 2,
                        child: Text("repeat_daily".tr()),
                      ),
                      DropdownMenuItem(
                        value: 3,
                        child: Text("repeat_weekly".tr()),
                      ),
                      DropdownMenuItem(
                        value: 4,
                        child: Text("repeat_interval".tr()),
                      ),
                    ],
                    onChanged: (v) {
                      setStateDialog(() {
                        repeat = v!;
                      });
                    },
                  ),

                  if (repeat == 4) ...[
                    const SizedBox(height: 12),

                    TextFormField(
                      controller: intervalController,
                      keyboardType: TextInputType.number,
                      decoration: InputDecoration(
                        labelText: "repeat_interval_minutes".tr(),
                      ),
                    ),
                  ],

                  const SizedBox(height: 12),

                  SwitchListTile(
                    value: isActive,
                    onChanged: (v) {
                      setStateDialog(() {
                        isActive = v;
                      });
                    },
                    title: Text("state_active".tr()),
                  ),
                ],
              ),
            ),
            actions: [
              ElevatedButton(
                onPressed: () async {
                  final intervalMinutes = repeat == 4
                      ? int.tryParse(intervalController.text)
                      : null;

                  await api.update(
                    c.id,
                    type,
                    startTime,
                    repeat,
                    intervalMinutes,
                    isActive,
                  );

                  if (!mounted) return;

                  Navigator.pop(context);
                  load();
                },
                child: Text("action_save".tr()),
              ),
            ],
          );
        },
      ),
    );
  }

  void deleteCommand(int id) async {
    await api.delete(id);
    load();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("label_commands".tr()),
        actions: [LanguageButton(
            onChanged: () {
              setState(() {});
            },
          ),],
      ),

      floatingActionButton: FloatingActionButton(
        onPressed: createCommand,
        child: const Icon(Icons.add),
      ),

      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.all(12),
            child: Row(
              children: [
                Expanded(
                  child: ElevatedButton(
                    onPressed: () => sendCommand(1),
                    child: Text("command_action_on".tr()),
                  ),
                ),
                const SizedBox(width: 10),
                Expanded(
                  child: ElevatedButton(
                    onPressed: () => sendCommand(2),
                    child: Text("command_action_off".tr()),
                  ),
                ),
              ],
            ),
          ),

          const Divider(),

          Expanded(
            child: commands.isEmpty
                ? Center(child: Text("no_commands").tr())
                : ListView.builder(
                    itemCount: commands.length,
                    itemBuilder: (_, i) {
                      final c = commands[i];

                      return Card(
                        child: ListTile(
                          title: Text(commandTypeToText(c.commandType)),
                          subtitle: Text(
                            "command_summary".tr(
                              args: [
                                repeatModeToText(c.repeatMode),
                                c.isActive
                                    ? "state_active".tr()
                                    : "state_off".tr(),
                              ],
                            ),
                          ),
                          trailing: Row(
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              IconButton(
                                icon: const Icon(Icons.edit),
                                onPressed: () => editCommand(c),
                              ),
                              IconButton(
                                icon: const Icon(Icons.delete),
                                onPressed: () => deleteCommand(c.id),
                              ),
                            ],
                          ),
                        ),
                      );
                    },
                  ),
          ),
        ],
      ),
    );
  }
}
