Arebis Mocking Library
======================

Manual Mocking
--------------




Remoting Mocking
----------------


Sample of code to put in APP.CONFIG:

<system.runtime.remoting>
	<application>
		<channels>
			<channel ref="http" useDefaultCredentials="true" port="0">
				<clientProviders>
					<provider type="Arebis.Mocking.Remoting.RemotingMockingSinkProvider, Arebis.Mocking" />
					<formatter ref="binary"/>
				</clientProviders>
			</channel>
		</channels>
	</application>
</system.runtime.remoting>


