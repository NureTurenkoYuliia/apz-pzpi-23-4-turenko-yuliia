import 'package:flutter/material.dart';
import '../../../device/data/datasources/device_api.dart';
import '../../../device/data/models/device_model.dart';
import '../../../device/data/models/sensor_data_model.dart';
import '../../../../core/utils/token_storage.dart';
import '../../../../core/network/api_client.dart';
import '../../../device/presentation/widgets/device_card.dart';
import '../../../rule/presentation/screens/alarm_rules_screen.dart';
import '../../../command/presentation/screens/commands_screen.dart';
import '../../../../shared/widgets/language_button.dart';
import 'package:easy_localization/easy_localization.dart';

class AquariumDetailsScreen extends StatefulWidget {
  final int aquariumId;
  final String name;

  const AquariumDetailsScreen({
    super.key,
    required this.aquariumId,
    required this.name,
  });

  @override
  State<AquariumDetailsScreen> createState() => _AquariumDetailsScreenState();
}

class _AquariumDetailsScreenState extends State<AquariumDetailsScreen> {
  List<Device> devices = [];
  Map<int, SensorData?> sensors = {};

  bool isLoading = true;
  late DeviceApi api;

  @override
  void initState() {
    super.initState();
    init();
  }

  Future<void> init() async {
    final token = await TokenStorage().getAccessToken();
    api = DeviceApi(ApiClient(token).dio);

    await load();
  }

  Future<void> load() async {
    setState(() => isLoading = true);

    try {
      devices = await api.getByAquarium(widget.aquariumId);

      sensors.clear();

      for (var d in devices) {
        try {
          sensors[d.id] = await api.getSensor(d.id);
        } catch (_) {}
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text("Error: $e")));
      }
    } finally {
      if (mounted) {
        setState(() => isLoading = false);
      }
    }
  }

  void deleteDevice(int id) async {
    await showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title: Text("delete_confirm".tr()),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: Text("action_cancel".tr()),
          ),
          ElevatedButton(
            onPressed: () async {
              await api.delete(id);

              if (!mounted) return;

              Navigator.pop(context);
              load();
            },
            child: Text("action_delete".tr()),
          ),
        ],
      ),
    );
  }

  void editDevice(Device d) async {
    int selectedStatus = d.deviceStatus;

    await showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title: Text("device_edit_title".tr()),
        content: DropdownButton<int>(
          value: selectedStatus,
          items: [
            DropdownMenuItem(value: 1, child: Text("state_on".tr())),
            DropdownMenuItem(value: 2, child: Text("state_off".tr())),
          ],
          onChanged: (v) => selectedStatus = v!,
        ),
        actions: [
          ElevatedButton(
            onPressed: () async {
              await api.update(d.id, d.deviceType, selectedStatus);

              if (!mounted) return;

              Navigator.pop(context);
              load();
            },
            child: Text("action_save".tr()),
          ),
        ],
      ),
    );
  }

  void createDeviceDialog() async {
    int? selectedType;

    await showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title: Text("create_device".tr()),
        content: DropdownButtonFormField<int>(
          hint: Text("device_select_type".tr()),
          items: [
            DropdownMenuItem(value: 1, child: Text("device_heater".tr())),
            DropdownMenuItem(value: 2, child: Text("device_light".tr())),
            DropdownMenuItem(value: 3, child: Text("device_oxygen_pump".tr())),
            DropdownMenuItem(value: 4, child: Text("device_filter".tr())),
            DropdownMenuItem(value: 5, child: Text("device_feeder".tr())),
          ],
          onChanged: (v) => selectedType = v,
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: Text("action_cancel".tr()),
          ),
          ElevatedButton(
            onPressed: () async {
              if (selectedType == null) return;

              await api.create(widget.aquariumId, selectedType!);

              if (!mounted) return;

              Navigator.pop(context);
              load();
            },
            child: Text("action_create".tr()),
          ),
        ],
      ),
    );
  }

  String deviceName(int type) {
    switch (type) {
      case 1:
        return "device_heater".tr();
      case 2:
        return "device_light".tr();
      case 3:
        return "device_oxygen_pump".tr();
      case 4:
        return "device_filter".tr();
      case 5:
        return "device_feeder".tr();
      default:
        return "device".tr();
    }
  }

  String deviceStatus(int status) {
    switch (status) {
      case 1:
        return "state_on".tr();
      case 2:
        return "state_off".tr();
      case 3:
        return "state_unknown".tr();
      default:
        return "state_unknown".tr();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(widget.name),
        actions: [
          LanguageButton(
            onChanged: () {
              setState(() {});
            },
          ),
        ],
      ),
      body: isLoading
          ? const Center(child: CircularProgressIndicator())
          : devices.isEmpty
          ? Center(child: Text("no_devices".tr()))
          : ListView.builder(
              itemCount: devices.length,
              itemBuilder: (_, i) {
                final d = devices[i];
                final sensor = sensors[d.id];

                return DeviceCard(
                  device: d,
                  sensor: sensor,
                  onAction: (value) {
                    if (value == 'edit') editDevice(d);
                    if (value == 'delete') deleteDevice(d.id);
                    if (value == 'rules') {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (_) => AlarmRulesScreen(deviceId: d.id),
                        ),
                      );
                    }

                    if (value == 'commands') {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (_) => CommandsScreen(deviceId: d.id),
                        ),
                      );
                    }
                  },
                );
              },
            ),
      floatingActionButton: FloatingActionButton(
        onPressed: createDeviceDialog,
        child: const Icon(Icons.add),
      ),
    );
  }
}
