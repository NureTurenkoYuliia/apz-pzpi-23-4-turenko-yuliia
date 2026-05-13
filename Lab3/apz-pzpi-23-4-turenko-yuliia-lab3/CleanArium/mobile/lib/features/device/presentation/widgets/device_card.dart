import 'package:flutter/material.dart';
import '../../data/models/device_model.dart';
import '../../data/models/sensor_data_model.dart';
import 'package:easy_localization/easy_localization.dart';

class DeviceCard extends StatelessWidget {
  final Device device;
  final SensorData? sensor;
  final Function(String) onAction;

  const DeviceCard({
    super.key,
    required this.device,
    required this.sensor,
    required this.onAction,
  });

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
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
      child: Padding(
        padding: const EdgeInsets.all(14),
        child: Row(
          children: [
            // LEFT
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    deviceName(device.deviceType),
                    style: const TextStyle(fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 4),
                  Text("${"label_status".tr()}: ${deviceStatus(device.deviceStatus)}"),
                ],
              ),
            ),
            SizedBox(
              width: 90,
              child: sensor != null
                  ? Text(
                      "${sensor!.value} ${sensor!.unit}",
                      textAlign: TextAlign.center,
                      overflow: TextOverflow.ellipsis,
                      style: const TextStyle(fontWeight: FontWeight.w500),
                    )
                  : Text("state_no_data".tr(), textAlign: TextAlign.center),
            ),
            
            const SizedBox(width: 6),
            PopupMenuButton<String>(
              onSelected: onAction,
              itemBuilder: (_) => [
                PopupMenuItem(value: 'edit', child: Text("action_edit".tr())),
                PopupMenuItem(value: 'delete', child: Text("action_delete".tr())),
                PopupMenuItem(value: 'rules', child: Text("label_alarm_rules".tr())),
                PopupMenuItem(value: 'commands', child: Text("label_commands".tr())),
                PopupMenuItem(value: 'analytics', child: Text("label_analytics".tr())),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
