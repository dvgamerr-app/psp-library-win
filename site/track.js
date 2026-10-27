// Traceway init. Does nothing unless the deploy workflow injected a DSN and the SDK.
(function () {
  var meta = document.querySelector('meta[name="traceway-dsn"]');
  if (!meta || !meta.content || !window.Traceway) return;
  try {
    Traceway.init(meta.content, { recordAllSessions: true });
    document.addEventListener('click', function (e) {
      var el = e.target.closest && e.target.closest('[data-track]');
      if (el) Traceway.recordAction('click', el.getAttribute('data-track'));
    }, true);
    window.addEventListener('pagehide', function () { Traceway.flush(1500); });
  } catch (err) { /* telemetry must never break the page */ }
})();
