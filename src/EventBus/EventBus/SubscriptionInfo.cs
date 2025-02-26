﻿namespace nagiashraf.CoursesApp.EventBus.EventBus;

public class SubscriptionInfo
{
    public Type HandlerType { get; }

    private SubscriptionInfo(Type handlerType)
    {
        HandlerType = handlerType;
    }

    public static SubscriptionInfo Typed(Type handlerType) =>
            new(handlerType);
}
