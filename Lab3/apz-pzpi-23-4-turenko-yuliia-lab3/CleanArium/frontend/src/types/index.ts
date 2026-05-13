export interface AuthResponseDto {
  accessToken: string;
  refreshToken: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  userName: string;
  email: string;
  password: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface LogoutRequest {
  refreshToken: string;
}

export enum UserRole {
  User = 'User',
  Moderator = 'Moderator',
  Admin = 'Admin',
}

export interface DecodedToken {
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier': string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress': string;
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': string;
  exp: number;
}

export interface PreviewUserDto {
  userId: number;
  email: string;
  lastLoginAt: string | null;
}

export interface UserDto {
  id: number;
  email: string;
  isBlocked: boolean;
  lastLoginAt: string | null;
  createdAt: string;
}

export interface InactiveUserDto {
  userId: number;
  email: string;
  lastLoginAt: string | null;
  aquariumsCount: number;
  activeAquariums: number;
  activeDevices: number;
}

export interface ModeratorDto {
  id: number;
  name: string;
  email: string;
  lastLoginAt: string | null;
  createdAt: string;
}

export interface SystemSettingsDto {
  maxAquariumsPerUser: number;
  maxDevicesPerAquarium: number;
  maxAlarmRulesPerDevice: number;
  maxScheduledCommandsPerDevice: number;
}

export interface UpdateSystemSettingsRequest {
  maxAquariumsPerUser: number;
  maxDevicesPerAquarium: number;
  maxAlarmRulesPerDevice: number;
  maxScheduledCommandsPerDevice: number;
}

export interface UserActivityDailyDto {
  date: string;
  actionsCount: number;
}

export enum CommandType {
  TurnOn = 1,
  TurnOff = 2,
  SetValue = 3,
  Calibrate = 4,
}

export interface CommandAlarmCorrelationDto {
  deviceId: number;
  commandType: CommandType;
  commandCount: number;
  alarmCount: number;
  avgDelayBetweenCommandAndAlarm: string;
  recommendation: string;
}

export enum DeviceType {
  Heater = 1,
  Light = 2,
  OxygenPump = 3,
  Filter = 4,
  Feeder = 5,
}

export enum DeviceStatus {
  On = 1,
  Off = 2,
  Unknown = 3,
}

export enum CommandStatus {
  Pending = 1,
  Sent = 2,
  Executed = 3,
  Failed = 4,
}

export enum ConditionType {
  Less = 1,
  Greater = 2,
  LessOrEqual = 3,
  GreaterOrEqual = 4,
  Equal = 5,
  NotEqual = 6,
}

export enum RepeatMode {
  None = 1,
  Daily = 2,
  Weekly = 3,
  Interval = 4,
}

export interface AquariumDto {
  id: number;
  userId: number;
  name: string;
  location: string | null;
  isActive: boolean;
}

export interface CreateAquariumRequest {
  name: string;
  location?: string;
}

export interface UpdateAquariumRequest {
  aquariumId: number;
  name: string;
  location?: string;
}

export interface DeviceDto {
  id: number;
  aquariumId: number;
  deviceType: DeviceType;
  deviceStatus: DeviceStatus;
}

export interface CreateDeviceRequest {
  aquariumId: number;
  deviceType: DeviceType;
  deviceStatus: DeviceStatus;
}

export interface UpdateDeviceRequest {
  deviceId: number;
  deviceType: DeviceType;
  deviceStatus: DeviceStatus;
}

export interface SensorDataDto {
  id: number;
  deviceId: number;
  value: number;
  unit: string;
  dateTime: string;
}

export interface ExecuteCommandRequest {
  commandType: CommandType;
  commandStatus: CommandStatus;
}

export interface AlarmRuleDto {
  id: number;
  deviceId: number;
  condition: ConditionType;
  threshold: number;
  unit: string;
  isActive: boolean;
}

export interface CreateAlarmRuleRequest {
  deviceId: number;
  condition: ConditionType;
  threshold: number;
  unit: string;
}

export interface UpdateAlarmRuleRequest {
  ruleId: number;
  condition: ConditionType;
  threshold: number;
  unit: string;
  isActive: boolean;
}

export interface AlarmRuleAnalysisDto {
  alarmRuleId: number;
  averageValue: number;
  trendPerDay: number;
  estimatedDaysToTrigger: number;
  recommendation: string;
}

export interface ScheduledCommandDto {
  id: number;
  deviceId: number;
  commandType: CommandType;
  startTime: string;
  repeatMode: RepeatMode;
  intervalMinutes: number | null;
  isActive: boolean;
}

export interface CreateScheduledCommandRequest {
  deviceId: number;
  commandType: CommandType;
  startTime: string;
  repeatMode: RepeatMode;
  intervalMinutes: number | null;
  isActive: boolean;
}

export interface UpdateScheduledCommandRequest {
  id: number;
  commandType: CommandType;
  startTime: string;
  repeatMode: RepeatMode;
  intervalMinutes: number | null;
  isActive: boolean;
}

export interface NotificationDto {
  id: number;
  title: string;
  content: string;
  isRead: boolean;
  createdAt: string;
}