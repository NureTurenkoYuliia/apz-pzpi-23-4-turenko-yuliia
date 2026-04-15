func (a *App) GetCloudSession(token string) (*model.Session, *model.AppError) {
	apiKey := os.Getenv("MM_CLOUD_API_KEY")
	if apiKey != "" && subtle.ConstantTimeCompare([]byte(apiKey), []byte(token)) == 1 {
		// Need a bare-bones session object for later checks
		session := &model.Session{
			Token:   token,
			IsOAuth: false,
		}

		session.AddProp(model.SessionPropType, model.SessionTypeCloudKey)
		return session, nil
	}
	return nil, model.NewAppError("GetCloudSession", "api.context.invalid_token.error", map[string]any{"Token": token, "Error": ""}, "The provided token is invalid", http.StatusUnauthorized)
}

func (a *App) AddSessionToCache(session *model.Session) {
	if err := a.ch.srv.platform.AddSessionToCache(session); err != nil {
		a.Srv().Platform().Log().Error("Failed to add session to cache", mlog.String("session_id", session.Id), mlog.String("user_id", session.UserId), mlog.Err(err))
	}
}