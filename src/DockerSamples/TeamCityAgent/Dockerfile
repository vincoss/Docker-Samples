FROM jetbrains/teamcity-agent AS teamcityagent

FROM teamcityagentcore:1.0.0-windows

COPY --from=teamcityagent ["/BuildAgent", "/BuildAgent"]

## NOTE: The agent is not startet as a windows service