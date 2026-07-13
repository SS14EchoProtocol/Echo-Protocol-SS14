#nullable enable
using System.Collections.Generic;
using System.Linq;
using Content.IntegrationTests.Fixtures;
<<<<<<< HEAD
=======
using Content.IntegrationTests.Fixtures.Attributes;
>>>>>>> wizzden/master
using Content.Server.Administration.Logs;
using Content.Server.Database;
using Content.Server.GameTicking;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Robust.Server.Player;

namespace Content.IntegrationTests.Tests.Administration.Logs;

[TestFixture]
[TestOf(typeof(AdminLogSystem))]
public sealed class AddTests : GameTest
{
    public override PoolSettings PoolSettings => new()
    {
        AdminLogsEnabled = true,
        DummyTicker = false,
        Connected = true
    };

    [SidedDependency(Side.Server)] private readonly IAdminLogManager _sAdminLogManager = null!;
    [SidedDependency(Side.Server)] private readonly IServerDbManager _sDbManager = null!;
    [SidedDependency(Side.Server)] private readonly GameTicker _sGameTicker = null!;
    [SidedDependency(Side.Server)] private readonly IPlayerManager _sPlayerManager = null!;

    [Test]
    public async Task AddAndGetSingleLog()
    {
<<<<<<< HEAD
        var pair = Pair;
        var server = pair.Server;
        var sEntities = server.ResolveDependency<IEntityManager>();

        var sAdminLogSystem = server.ResolveDependency<IAdminLogManager>();

        var guid = Guid.NewGuid();

        await pair.CreateTestMap();
        var coordinates = pair.TestMap!.GridCoords;
        await server.WaitPost(() =>
=======
        var guid = Guid.NewGuid();

        await Pair.CreateTestMap();
        var coordinates = Pair.TestMap!.GridCoords;
        await Server.WaitPost(() =>
>>>>>>> wizzden/master
        {
            var entity = SSpawnAtPosition(null, coordinates);

            _sAdminLogManager.Add(LogType.Unknown, $"{entity:Entity} test log: {guid}");
        });

        await PoolManager.WaitUntil(Server, async () =>
        {
            var logs = _sAdminLogManager.CurrentRoundJson(new LogFilter
            {
                Search = guid.ToString()
            });

            await foreach (var json in logs)
            {
                var root = json.RootElement;

                // camelCased automatically
                Assert.That(root.TryGetProperty("entity", out _), Is.True);

                json.Dispose();

                return true;
            }

            return false;
        });
    }

    [Test]
    public async Task AddAndGetUnformattedLog()
    {
<<<<<<< HEAD
        var pair = Pair;
        var server = pair.Server;

        var sDatabase = server.ResolveDependency<IServerDbManager>();
        var sEntities = server.ResolveDependency<IEntityManager>();
        var sSystems = server.ResolveDependency<IEntitySystemManager>();

        var sAdminLogSystem = server.ResolveDependency<IAdminLogManager>();
        var sGamerTicker = sSystems.GetEntitySystem<GameTicker>();

=======
>>>>>>> wizzden/master
        var guid = Guid.NewGuid();

        var testMap = await Pair.CreateTestMap();
        var coordinates = testMap.GridCoords;
        await Server.WaitPost(() =>
        {
            var entity = SSpawnAtPosition(null, coordinates);

            _sAdminLogManager.Add(LogType.Unknown, $"{entity} test log: {guid}");
        });

        SharedAdminLog log = default;

        await PoolManager.WaitUntil(Server, async () =>
        {
            var logs = await _sAdminLogManager.CurrentRoundLogs(new LogFilter
            {
                Search = guid.ToString()
            });

            if (logs.Count == 0)
            {
                return false;
            }

            log = logs.First();
            return true;
        });

        var filter = new LogFilter
        {
            Round = _sGameTicker.RoundId,
            Search = log.Message,
            Types = new HashSet<LogType> { log.Type },
        };

        await foreach (var json in _sDbManager.GetAdminLogsJson(filter))
        {
            var root = json.RootElement;

            Assert.Multiple(() =>
            {
                Assert.That(root.TryGetProperty("entity", out _), Is.True);
                Assert.That(root.TryGetProperty("guid", out _), Is.True);
            });

            json.Dispose();
        }
    }

    [Test]
    [TestCase(500)]
    public async Task BulkAddLogs(int amount)
    {
<<<<<<< HEAD
        var pair = Pair;
        var server = pair.Server;

        var sEntities = server.ResolveDependency<IEntityManager>();
        var sAdminLogSystem = server.ResolveDependency<IAdminLogManager>();

        var testMap = await pair.CreateTestMap();
=======
        var testMap = await Pair.CreateTestMap();
>>>>>>> wizzden/master
        var coordinates = testMap.GridCoords;
        await Server.WaitPost(() =>
        {
            var entity = SSpawnAtPosition(null, coordinates);

            for (var i = 0; i < amount; i++)
            {
                _sAdminLogManager.Add(LogType.Unknown, $"{entity:Entity} test log.");
            }
        });

        await PoolManager.WaitUntil(Server, async () =>
        {
            var messages = await _sAdminLogManager.CurrentRoundLogs();
            return messages.Count >= amount;
        });
    }

    [Test]
    public async Task AddPlayerSessionLog()
    {
<<<<<<< HEAD
        var pair = Pair;
        var server = pair.Server;

        var sPlayers = server.ResolveDependency<IPlayerManager>();

        var sAdminLogSystem = server.ResolveDependency<IAdminLogManager>();
=======
>>>>>>> wizzden/master
        Guid playerGuid = default;

        await Server.WaitPost(() =>
        {
            var player = _sPlayerManager.Sessions.First();
            playerGuid = player.UserId;

            Assert.DoesNotThrow(() =>
            {
                _sAdminLogManager.Add(LogType.Unknown, $"{player:Player} test log.");
            });
        });

        await PoolManager.WaitUntil(Server, async () =>
        {
            var logs = await _sAdminLogManager.CurrentRoundLogs();
            if (logs.Count == 0)
            {
                return false;
            }

            Assert.That(logs.First().Players, Does.Contain(playerGuid));
            return true;
        });
    }

    [Test]
    public async Task DuplicatePlayerDoesNotThrowTest()
    {
<<<<<<< HEAD
        var pair = Pair;
        var server = pair.Server;

        var sPlayers = server.ResolveDependency<IPlayerManager>();
        var sAdminLogSystem = server.ResolveDependency<IAdminLogManager>();

        var guid = Guid.NewGuid();

        await server.WaitPost(() =>
        {
            var player = sPlayers.Sessions.Single();

            sAdminLogSystem.Add(LogType.Unknown, $"{player} {player} test log: {guid}");
        });

        await PoolManager.WaitUntil(server, async () =>
        {
            var logs = await sAdminLogSystem.CurrentRoundLogs(new LogFilter
=======
        var guid = Guid.NewGuid();

        await Server.WaitPost(() =>
        {
            var player = _sPlayerManager.Sessions.Single();

            _sAdminLogManager.Add(LogType.Unknown, $"{player} {player} test log: {guid}");
        });

        await PoolManager.WaitUntil(Server, async () =>
        {
            var logs = await _sAdminLogManager.CurrentRoundLogs(new LogFilter
>>>>>>> wizzden/master
            {
                Search = guid.ToString()
            });

            if (logs.Count == 0)
            {
                return false;
            }

            return true;
        });
    }

    [Test]
    public async Task DuplicatePlayerIdDoesNotThrowTest()
    {
<<<<<<< HEAD
        var pair = Pair;
        var server = pair.Server;

        var sPlayers = server.ResolveDependency<IPlayerManager>();

        var sAdminLogSystem = server.ResolveDependency<IAdminLogManager>();

        var guid = Guid.NewGuid();

        await server.WaitPost(() =>
        {
            var player = sPlayers.Sessions.Single();

            sAdminLogSystem.Add(LogType.Unknown, $"{player:first} {player:second} test log: {guid}");
        });

        await PoolManager.WaitUntil(server, async () =>
        {
            var logs = await sAdminLogSystem.CurrentRoundLogs(new LogFilter
=======
        var guid = Guid.NewGuid();

        await Server.WaitPost(() =>
        {
            var player = _sPlayerManager.Sessions.Single();

            _sAdminLogManager.Add(LogType.Unknown, $"{player:first} {player:second} test log: {guid}");
        });

        await PoolManager.WaitUntil(Server, async () =>
        {
            var logs = await _sAdminLogManager.CurrentRoundLogs(new LogFilter
>>>>>>> wizzden/master
            {
                Search = guid.ToString()
            });

            if (logs.Count == 0)
            {
                return false;
            }

            return true;
        });
    }
}

public sealed class PreRoundAddTests : GameTest
{
    public override PoolSettings PoolSettings => new PoolSettings
    {
        Dirty = true,
        InLobby = true,
        AdminLogsEnabled = true
    };

<<<<<<< HEAD
    [Test]
    public async Task PreRoundAddAndGetSingle()
    {
        var pair = Pair;
        var server = pair.Server;

        var sDatabase = server.ResolveDependency<IServerDbManager>();
        var sSystems = server.ResolveDependency<IEntitySystemManager>();

        var sAdminLogSystem = server.ResolveDependency<IAdminLogManager>();
        var sGamerTicker = sSystems.GetEntitySystem<GameTicker>();

=======
    [SidedDependency(Side.Server)] private readonly IAdminLogManager _sAdminLogManager = null!;
    [SidedDependency(Side.Server)] private readonly IServerDbManager _sDbManager = null!;
    [SidedDependency(Side.Server)] private readonly GameTicker _sGameTicker = null!;

    [Test]
    public async Task PreRoundAddAndGetSingle()
    {
>>>>>>> wizzden/master
        var guid = Guid.NewGuid();

        await Server.WaitPost(() =>
        {
            _sAdminLogManager.Add(LogType.Unknown, $"test log: {guid}");
        });

        await Server.WaitPost(() =>
        {
            _sGameTicker.StartRound(true);
        });

        SharedAdminLog log = default;

        await PoolManager.WaitUntil(Server, async () =>
        {
            var logs = await _sAdminLogManager.CurrentRoundLogs(new LogFilter
            {
                Search = guid.ToString()
            });

            if (logs.Count == 0)
            {
                return false;
            }

            log = logs.First();
            return true;
        });

        var filter = new LogFilter
        {
            Round = _sGameTicker.RoundId,
            Search = log.Message,
            Types = new HashSet<LogType> { log.Type },
        };

        await foreach (var json in _sDbManager.GetAdminLogsJson(filter))
        {
            var root = json.RootElement;

            Assert.That(root.TryGetProperty("guid", out _), Is.True);

            json.Dispose();
        }
    }

}
